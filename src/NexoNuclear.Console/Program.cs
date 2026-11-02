using System;
using System.Collections.Generic;
using System.Globalization;
using NexoNuclear.Models;
using NexoNuclear.Services;

namespace NexoNuclear
{
    internal class Program
    {
        private static RepositorioAtividadesJson Repositorio = null!;
        private static readonly CalculadoraDosimetriaService Calculadora = new();

        static void Main(string[] args)
        {
            Console.OutputEncoding = System.Text.Encoding.UTF8;
            try
            {
                Repositorio = new RepositorioAtividadesJson();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[Erro ao iniciar] {ex.Message}");
                Environment.ExitCode = 1;
                return;
            }
            bool executando = true;

            while (executando)
            {
                try
                {
                    ExibirMenuPrincipal();
                    Console.Write("Escolha uma opção: ");
                    string? opcao = Console.ReadLine()?.Trim();
                    if (opcao == null) break;

                    Console.WriteLine();
                    switch (opcao)
                    {
                        case "1":
                            ListarAtividades();
                            break;
                        case "2":
                            BuscarAtividades();
                            break;
                        case "3":
                            CadastrarNovaAtividade();
                            break;
                        case "4":
                            ExecutarSimuladorDosimetria();
                            break;
                        case "5":
                            ExibirSobreProjetoNuclear();
                            break;
                        case "0":
                            executando = false;
                            Console.ForegroundColor = ConsoleColor.Green;
                            Console.WriteLine("Obrigado por utilizar o NexoNuclear! Até logo.");
                            Console.ResetColor();
                            break;
                        default:
                            Console.ForegroundColor = ConsoleColor.Yellow;
                            Console.WriteLine("[!] Opção inválida. Digite um número de 0 a 5.");
                            Console.ResetColor();
                            break;
                    }
                }
                catch (EndOfStreamException)
                {
                    break;
                }
                catch (Exception ex)
                {
                    Console.ForegroundColor = ConsoleColor.Red;
                    Console.WriteLine($"[Erro Inesperado] {ex.Message}");
                    Console.ResetColor();
                }

                if (executando)
                {
                    Console.WriteLine();
                    Console.Write("Pressione ENTER para voltar ao menu...");
                    Console.ReadLine();
                }
            }
        }

        private static void ExibirMenuPrincipal()
        {
            if (!Console.IsOutputRedirected) Console.Clear();
            Console.ForegroundColor = ConsoleColor.Cyan;
            Console.WriteLine("================================================================================");
            Console.WriteLine("       NEXONUCLEAR - MÓDULO .NET DE DOSIMETRIA E TECNOLOGIA NUCLEAR            ");
            Console.WriteLine("               Desafio FIAP / Marinha do Brasil / AMAZUL 2026                   ");
            Console.WriteLine("================================================================================");
            Console.ResetColor();
            Console.WriteLine(" 1. Listar todas as atividades e fontes de radiação");
            Console.WriteLine(" 2. Buscar atividade por nome, categoria ou palavra-chave");
            Console.WriteLine(" 3. Cadastrar nova atividade radiológica");
            Console.WriteLine(" 4. Simulador Interativo de Exposição Radiológica Pessoal");
            Console.WriteLine(" 5. Sobre a Tecnologia Nuclear e o Programa Nuclear da Marinha (PNM)");
            Console.WriteLine(" 0. Sair");
            Console.WriteLine("--------------------------------------------------------------------------------");
        }

        private static void ListarAtividades()
        {
            Console.ForegroundColor = ConsoleColor.Yellow;
            Console.WriteLine("--- CATÁLOGO DE ATIVIDADES E FONTES DE RADIAÇÃO ---");
            Console.ResetColor();

            var lista = Repositorio.ListarTodas();
            if (lista.Count == 0)
            {
                Console.WriteLine("Nenhuma atividade cadastrada no momento.");
                return;
            }

            foreach (var item in lista)
            {
                ExibirCardAtividade(item);
            }
        }

        private static void BuscarAtividades()
        {
            Console.ForegroundColor = ConsoleColor.Yellow;
            Console.WriteLine("--- CONSULTA DE ATIVIDADES RADIOLÓGICAS ---");
            Console.ResetColor();
            Console.Write("Digite o termo de busca (nome, categoria ou descrição): ");
            string? termo = Console.ReadLine();

            var resultados = Repositorio.BuscarPorTermo(termo ?? string.Empty);
            Console.WriteLine();
            Console.WriteLine($"Encontrado(s) {resultados.Count} registro(s):");

            if (resultados.Count == 0)
            {
                Console.ForegroundColor = ConsoleColor.DarkYellow;
                Console.WriteLine("Nenhuma atividade encontrada com o termo informado.");
                Console.ResetColor();
                return;
            }

            foreach (var item in resultados)
            {
                ExibirCardAtividade(item);
            }
        }

        private static void CadastrarNovaAtividade()
        {
            Console.ForegroundColor = ConsoleColor.Yellow;
            Console.WriteLine("--- CADASTRO DE NOVA ATIVIDADE RADIOLÓGICA ---");
            Console.ResetColor();

            try
            {
                Console.Write("Nome da atividade ionizante (ex: Raio-X de tórax PA / Voo hipotético): ");
                string? nome = Console.ReadLine();

                Console.Write("Categoria (ex: Radiologia diagnóstica, Radiação cósmica em voo): ");
                string? categoria = Console.ReadLine();

                Console.Write("Descrição / Justificativa educativa: ");
                string? descricao = Console.ReadLine();

                Console.Write("Dose média por evento em microsieverts (µSv) [ex: 20 ou 0.1]: ");
                string? doseStr = Console.ReadLine()?.Replace(',', '.');
                if (!double.TryParse(doseStr, NumberStyles.Float, CultureInfo.InvariantCulture, out double dose) || !double.IsFinite(dose) || dose <= 0)
                {
                    Console.ForegroundColor = ConsoleColor.Red;
                    Console.WriteLine("[Erro] Valor de dose inválido. Deve ser um número finito maior que zero.");
                    Console.ResetColor();
                    return;
                }

                Console.Write("Frequência estimada no ano (quantas vezes por ano): ");
                string? freqStr = Console.ReadLine();
                if (!int.TryParse(freqStr, out int freq) || freq <= 0)
                {
                    Console.ForegroundColor = ConsoleColor.Red;
                    Console.WriteLine("[Erro] Frequência anual inválida. Deve ser um número inteiro maior que zero.");
                    Console.ResetColor();
                    return;
                }

                var novaAtividade = new AtividadeRadiologica(0, nome ?? string.Empty, categoria ?? "Geral", descricao ?? "Sem descrição", dose, freq);
                Repositorio.Cadastrar(novaAtividade);

                Console.ForegroundColor = ConsoleColor.Green;
                Console.WriteLine($"[Sucesso] Atividade '{novaAtividade.Nome}' cadastrada com ID #{novaAtividade.Id} e salva com sucesso!");
                Console.ResetColor();
            }
            catch (ArgumentException ex)
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine($"[Falha de Validação] {ex.Message}");
                Console.ResetColor();
            }
            catch (Exception ex)
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine($"[Erro ao Cadastrar] {ex.Message}");
                Console.ResetColor();
            }
        }

        private static void ExecutarSimuladorDosimetria()
        {
            Console.WriteLine("--- SIMULADOR EDUCATIVO: CONTEXTOS SEPARADOS ---");
            Console.WriteLine("Estimativas ilustrativas. Não é avaliação médica ou de conformidade regulatória.");
            Console.Write("Quantas radiografias de tórax PA no ano? ");
            int raiox = LerInteiroNaoNegativo();
            Console.Write("Quantas tomografias de abdômen no ano? ");
            int tomografias = LerInteiroNaoNegativo();
            Console.Write("Quantas horas de voo comercial no ano? ");
            int horasVoo = LerInteiroNaoNegativo();
            Console.Write("Quantas bananas por semana (apenas exemplo alimentar)? ");
            int bananasSemana = LerInteiroNaoNegativo();

            var resultado = Calculadora.Simular(raiox, tomografias, horasVoo, bananasSemana);
            Console.WriteLine("================ RESULTADO EDUCATIVO ================");
            Console.WriteLine($"Exames médicos: {resultado.DoseMedicaMicrosievert:N2} µSv/ano (estimativa típica).");
            Console.WriteLine("Parâmetros: tórax PA = 20 µSv; TC de abdômen = 8.000 µSv. Fonte: FDA.");
            Console.WriteLine("A dose real depende do exame, paciente, equipamento e protocolo.");
            Console.WriteLine($"Voos: {resultado.DoseVoosMicrosievert:N2} µSv/ano (hipótese didática de 5 µSv/h).");
            Console.WriteLine("Não representa uma rota real: altitude, latitude e duração alteram a exposição.");
            Console.WriteLine($"Alimentação: {resultado.BananasPorAno} bananas/ano (52 semanas ilustrativas).");
            Console.WriteLine("Não convertemos bananas em dose anual acumulada ou em indicação de risco.");
            Console.WriteLine("Não somamos esses contextos nem acrescentamos uma média de fundo natural.");
            Console.WriteLine("Não aplicamos o limite de exposição do público a exames ou fundo natural.");
            Console.WriteLine("O resultado não recomenda nem contraindica exames. Converse com a equipe de saúde.");
            Console.WriteLine("Fontes e limitações: docs/fontes-e-limitacoes.md (ANSN 3.01 e FDA).");
        }

        private static void ExibirSobreProjetoNuclear()
        {
            Console.WriteLine("--- TECNOLOGIA NUCLEAR E EDUCAÇÃO ---");
            Console.WriteLine("O Challenge FIAP/AMAZUL/Marinha propõe aproximar a sociedade das aplicações");
            Console.WriteLine("da tecnologia nuclear em saúde, indústria, agricultura, ciência e energia.");
            Console.WriteLine("Esta PoC demonstra cadastro, consulta e estimativas aritméticas por contexto.");
            Console.WriteLine("Radiografias e tomografias são radiologia diagnóstica, não medicina nuclear.");
            Console.WriteLine("Não mede doses, não classifica segurança e não certifica instalações do PNM.");
            Console.WriteLine("Fontes e limitações: docs/fontes-e-limitacoes.md.");
        }

        private static void ExibirCardAtividade(AtividadeRadiologica item)
        {
            // Cadastro educativo: categoria livre não determina enquadramento regulatório.

            Console.WriteLine($"[ID #{item.Id}] {item.Nome} ({item.Categoria})");
            Console.WriteLine($"    Descrição: {item.Descricao}");
            Console.WriteLine($"    Dose por evento: {item.DoseMicrosievert:N2} µSv | Frequência/Ano: {item.FrequenciaAnual}x");
            Console.WriteLine($"    Estimativa aritmética anual: {item.DoseTotalAnual:N2} µSv");
            Console.WriteLine("    Dose por evento x frequência; não é avaliação médica ou regulatória.");
            Console.WriteLine(new string('-', 80));
        }

        private static int LerInteiroNaoNegativo()
        {
            while (true)
            {
                string? entrada = Console.ReadLine();
                if (entrada == null) throw new EndOfStreamException("Entrada encerrada.");
                if (int.TryParse(entrada, out int valor) && valor >= 0)
                {
                    return valor;
                }
                Console.ForegroundColor = ConsoleColor.Yellow;
                Console.Write("[!] Digite um número inteiro válido (>= 0): ");
                Console.ResetColor();
            }
        }
    }
}
