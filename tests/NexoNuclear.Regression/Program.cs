using NexoNuclear.Models;
using NexoNuclear.Services;

int falhas = 0;
int total = 0;
void Testar(string nome, Action teste)
{
    total++;
    try { teste(); Console.WriteLine($"PASS {nome}"); }
    catch (Exception ex) { falhas++; Console.WriteLine($"FAIL {nome}: {ex.Message}"); }
}
void Exigir(bool condicao, string mensagem) { if (!condicao) throw new Exception(mensagem); }
void Rejeitar(Action acao)
{
    try { acao(); } catch (ArgumentException) { return; }
    throw new Exception("Entrada inválida foi aceita.");
}
foreach (double dose in new[] { double.NaN, double.PositiveInfinity, double.NegativeInfinity, 0.0, -1.0 })
    Testar($"Rejeitar dose {dose}", () => Rejeitar(() => AtividadeRadiologica.Validar("Teste", dose, 1)));
Testar("Rejeitar overflow anual", () => Rejeitar(() => AtividadeRadiologica.Validar("Teste", double.MaxValue, 2)));
Testar("Rejeitar nome curto", () => Rejeitar(() => AtividadeRadiologica.Validar("ab", 1, 1)));
Testar("Rejeitar frequência zero", () => Rejeitar(() => AtividadeRadiologica.Validar("Teste", 1, 0)));
string pasta = Path.Combine(Environment.GetEnvironmentVariable("TMPDIR") ?? AppContext.BaseDirectory, "NexoRegression-" + Guid.NewGuid());
Directory.CreateDirectory(pasta);
try
{
    foreach (string conteudo in new[] { "{invalido", "null", "[null]", "[{\"Id\":1,\"Nome\":\"Teste\",\"DoseMicrosievert\":-1,\"FrequenciaAnual\":1}]" })
        Testar($"Preservar JSON inválido {conteudo}", () =>
        {
            string arquivo = Path.Combine(pasta, Guid.NewGuid() + ".json");
            File.WriteAllText(arquivo, conteudo);
            bool rejeitado = false;
            try { _ = new RepositorioAtividadesJson(arquivo); }
            catch (InvalidOperationException) { rejeitado = true; }
            Exigir(File.ReadAllText(arquivo) == conteudo, "Arquivo original foi alterado.");
            Exigir(rejeitado, "Deveria informar erro ao abrir dados inválidos.");
        });
    Testar("Preservar catálogo vazio", () =>
    {
        string arquivo = Path.Combine(pasta, "vazio.json"); File.WriteAllText(arquivo, "[]");
        Exigir(new RepositorioAtividadesJson(arquivo).ListarTodas().Count == 0, "Seeds indevidos.");
        Exigir(File.ReadAllText(arquivo) == "[]", "Arquivo vazio foi regravado.");
    });
    Testar("Falha de gravação não confirma memória ou ID", () =>
    {
        string arquivo = Path.Combine(pasta, "bloqueado.json");
        var repo = new RepositorioAtividadesJson(arquivo);
        int antes = repo.ListarTodas().Count;
        byte[] original = File.ReadAllBytes(arquivo);
        var nova = new AtividadeRadiologica(0, "Cadastro teste", "Teste", "Teste", 2, 5);
        using (var bloqueio = new FileStream(arquivo, FileMode.Open, FileAccess.Read, FileShare.Read))
        {
            bool falhou = false;
            try { repo.Cadastrar(nova); } catch (InvalidOperationException) { falhou = true; }
            Exigir(falhou, "Falha de IO não propagada.");
            Exigir(repo.ListarTodas().Count == antes && nova.Id == 0, "Cadastro fantasma ou ID alterado.");
        }
        Exigir(File.ReadAllBytes(arquivo).SequenceEqual(original), "Arquivo anterior alterado.");
        Exigir(!Directory.GetFiles(pasta, "*.tmp").Any(), "Temporário abandonado.");
    });
    Testar("Cadastro, busca, recarga e backup", () =>
    {
        string arquivo = Path.Combine(pasta, "cadastro.json"); var repo = new RepositorioAtividadesJson(arquivo);
        byte[] original = File.ReadAllBytes(arquivo);
        repo.Cadastrar(new AtividadeRadiologica(0, "Registro teste", "Categoria teste", "Descrição especial", 2, 5));
        var recarga = new RepositorioAtividadesJson(arquivo);
        Exigir(recarga.BuscarPorTermo("REGISTRO TESTE").Single().DoseTotalAnual == 10, "Recarga incorreta.");
        Exigir(recarga.BuscarPorTermo("categoria teste").Count == 1 && recarga.BuscarPorTermo("especial").Count == 1, "Busca incorreta.");
        Exigir(File.ReadAllBytes(arquivo + ".bak").SequenceEqual(original), "Backup anterior ausente.");
    });
    Testar("Catálogo inicial sem equivalências alimentares ou dose de usina sem fonte", () =>
    {
        var repo = new RepositorioAtividadesJson(Path.Combine(pasta, "fontes.json"));
        var dados = repo.ListarTodas();
        Exigir(dados.Count == 5, "Catálogo inicial incompleto.");
        Exigir(dados.All(a => !a.Nome.Contains("Banana") && !a.Nome.Contains("Angra")), "Exemplo sem fundamento permaneceu.");
        Exigir(dados.Where(a => a.Nome.Contains("Tomografia")).All(a => a.Categoria == "Radiologia diagnóstica"), "TC classificada como medicina nuclear.");
    });
    Testar("Consultas não expõem objetos internos mutáveis", () =>
    {
        var repo = new RepositorioAtividadesJson(Path.Combine(pasta, "copias.json"));
        int id = repo.ListarTodas()[0].Id;
        repo.ListarTodas()[0].DoseMicrosievert = double.NaN;
        repo.BuscarPorId(id)!.Nome = "";
        repo.BuscarPorTermo("")[0].FrequenciaAnual = 0;
        var registro = repo.BuscarPorId(id)!;
        Exigir(double.IsFinite(registro.DoseMicrosievert) && registro.Nome.Length >= 3 && registro.FrequenciaAnual > 0,
            "Referência externa alterou o repositório sem gravação.");
    });
}
finally { Directory.Delete(pasta, true); }
Testar("Simular separa exames, voos e alimentação sem limite regulatório", () =>
{
    var calculadora = new CalculadoraDosimetriaService();
    var resultado = calculadora.Simular(1, 1, 10, 7);
    Exigir(resultado.DoseMedicaMicrosievert == 8020.0, "Dose médica incorreta.");
    Exigir(resultado.DoseVoosMicrosievert == 50.0, "Dose de voos incorreta.");
    Exigir(resultado.BananasPorAno == 364L, "Contagem alimentar incorreta.");
    Exigir(typeof(CalculadoraDosimetriaService).GetMethod("ClassificarRiscoAnual") == null,
        "Classificador de risco indevido ainda exposto.");
    Exigir(typeof(CalculadoraDosimetriaService).GetMethod("CalcularPorcentagemLimiteCnen") == null,
        "Comparação regulatória indevida ainda exposta.");
});
string ExecutarConsole(string entrada, string? conteudoInicial = null, bool esperarFalha = false)
{
    string execucao = Path.Combine(Environment.GetEnvironmentVariable("TMPDIR") ?? AppContext.BaseDirectory,
        "NexoConsole-" + Guid.NewGuid());
    Directory.CreateDirectory(execucao);
    try
    {
        foreach (string extensao in new[] { ".dll", ".deps.json", ".runtimeconfig.json" })
            File.Copy(Path.Combine(AppContext.BaseDirectory, "NexoNuclear.Console" + extensao),
                Path.Combine(execucao, "NexoNuclear.Console" + extensao));
        string dados = Path.Combine(execucao, "atividades_nucleares.json");
        if (conteudoInicial != null) File.WriteAllText(dados, conteudoInicial);
        var inicio = new System.Diagnostics.ProcessStartInfo("dotnet")
        {
            RedirectStandardInput = true, RedirectStandardOutput = true,
            RedirectStandardError = true, UseShellExecute = false,
            WorkingDirectory = execucao
        };
        inicio.ArgumentList.Add(Path.Combine(execucao, "NexoNuclear.Console.dll"));
        using var processo = System.Diagnostics.Process.Start(inicio)!;
        var saida = processo.StandardOutput.ReadToEndAsync();
        var erro = processo.StandardError.ReadToEndAsync();
        processo.StandardInput.Write(entrada); processo.StandardInput.Close();
        if (!processo.WaitForExit(5000))
        {
            processo.Kill(entireProcessTree: true); processo.WaitForExit();
            throw new Exception("Console não encerrou: loop ou entrada bloqueada.");
        }
        string texto = saida.GetAwaiter().GetResult() + erro.GetAwaiter().GetResult();
        Exigir(esperarFalha ? processo.ExitCode != 0 : processo.ExitCode == 0, "Código de saída inesperado: " + texto);
        if (conteudoInicial != null)
            Exigir(File.ReadAllText(dados) == conteudoInicial, "Console sobrescreveu o arquivo existente.");
        return texto;
    }
    finally { Directory.Delete(execucao, true); }
}
Testar("Console informa falha de inicialização e preserva JSON", () =>
{
    string texto = ExecutarConsole("", "{quebrado", esperarFalha: true);
    Exigir(texto.Contains("[Erro ao iniciar]") && texto.Contains("preservado"), "Erro de inicialização não tratado.");
    Exigir(!texto.Contains("Unhandled exception"), "Exceção sem tratamento.");
});
Testar("Console encerra em EOF", () => ExecutarConsole(""));
Testar("Simulador rejeita quantidades negativas", () =>
{
    var calc = new CalculadoraDosimetriaService();
    Rejeitar(() => calc.Simular(-1, 0, 0, 0));
    Rejeitar(() => calc.Simular(0, -1, 0, 0));
    Rejeitar(() => calc.Simular(0, 0, -1, 0));
    Rejeitar(() => calc.Simular(0, 0, 0, -1));
});
Testar("Simulador aceita zero sem inventar dose de fundo", () =>
{
    Exigir(new CalculadoraDosimetriaService().Simular(0, 0, 0, 0) == new ResultadoSimulacao(0, 0, 0), "Dose indevida.");
});
Testar("Simulador não estoura contagem alimentar", () =>
{
    var resultado = new CalculadoraDosimetriaService().Simular(int.MaxValue, int.MaxValue, int.MaxValue, int.MaxValue);
    Exigir(resultado.BananasPorAno == (long)int.MaxValue * 52 && double.IsFinite(resultado.DoseMedicaMicrosievert), "Overflow.");
});
Testar("Console lista e busca sem classificação regulatória", () =>
{
    string texto = ExecutarConsole("1\n\n2\nTomografia\n\n5\n\n0\n");
    Exigir(texto.Contains("Encontrado(s) 2 registro(s)") && texto.Contains("Até logo"), "Listagem, busca ou saída falhou.");
    Exigir(!texto.Contains("% do limite") && !texto.Contains("Classificação de Segurança"), "Alerta regulatório indevido.");
});
Testar("Console simula por contextos e repete entrada inválida", () =>
{
    string texto = ExecutarConsole("4\ntexto\n-1\n1\n1\n10\n7\n\n0\n");
    Exigir(texto.Contains("Digite um número inteiro válido") && texto.Contains("RESULTADO EDUCATIVO"), "Validação interativa falhou.");
    Exigir(texto.Contains("Exames médicos:") && texto.Contains("Voos:") && texto.Contains("364 bananas/ano"), "Contextos não separados.");
    Exigir(!texto.Contains("% do limite") && !texto.Contains("Dose Total Estimada"), "Agregação indevida.");
});
Testar("Console cadastra com vírgula e consulta", () =>
{
    string texto = ExecutarConsole("3\nTeste decimal\nTeste\nRegistro de teste\n0,5\n2\n\n2\nTeste decimal\n\n0\n");
    Exigir(texto.Contains("[Sucesso]") && texto.Contains("Encontrado(s) 1 registro(s)"), "Cadastro/consulta no console falhou.");
});
Testar("Console rejeita NaN sem confirmar cadastro", () =>
{
    string texto = ExecutarConsole("3\nTeste NaN\nTeste\nTeste\nNaN\n\n0\n");
    Exigir(texto.Contains("Valor de dose inválido") && !texto.Contains("[Sucesso]"), "Dose NaN aceita pelo console.");
});
Testar("Console trata opção inválida", () =>
{
    string texto = ExecutarConsole("opcao\n\n0\n");
    Exigir(texto.Contains("Opção inválida") && texto.Contains("Até logo"), "Menu não recuperou.");
});
Testar("Console encerra em EOF durante simulação", () => ExecutarConsole("4\n"));
Console.WriteLine($"Resultado: {total - falhas}/{total} testes passaram; {falhas} falhas.");
Environment.ExitCode = falhas == 0 ? 0 : 1;
