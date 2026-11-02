using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.Json;
using NexoNuclear.Models;

namespace NexoNuclear.Services
{
    /// <summary>
    /// Repositório de persistência em arquivo JSON com tratamento de exceções
    /// e carga inicial de dados (seeds) sobre tecnologia nuclear e dosimetria cotidiana.
    /// </summary>
    public class RepositorioAtividadesJson
    {
        private readonly string _caminhoArquivo;
        private readonly List<AtividadeRadiologica> _atividades = new();
        private readonly JsonSerializerOptions _jsonOptions = new()
        {
            WriteIndented = true
        };

        public RepositorioAtividadesJson(string? caminhoArquivo = null)
        {
            _caminhoArquivo = caminhoArquivo ?? Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "atividades_nucleares.json");
            CarregarOuInicializar();
        }

        public IReadOnlyList<AtividadeRadiologica> ListarTodas()
        {
            return _atividades.OrderBy(a => a.Id).Select(Copiar).ToList().AsReadOnly();
        }

        public AtividadeRadiologica? BuscarPorId(int id)
        {
            var atividade = _atividades.FirstOrDefault(a => a.Id == id);
            return atividade == null ? null : Copiar(atividade);
        }

        public List<AtividadeRadiologica> BuscarPorTermo(string termo)
        {
            if (string.IsNullOrWhiteSpace(termo))
            {
                return ListarTodas().ToList();
            }

            string t = termo.Trim().ToLowerInvariant();
            return _atividades
                .Where(a => a.Nome.ToLowerInvariant().Contains(t) ||
                            a.Categoria.ToLowerInvariant().Contains(t) ||
                            a.Descricao.ToLowerInvariant().Contains(t))
                .OrderBy(a => a.Nome)
                .Select(Copiar)
                .ToList();
        }

        public void Cadastrar(AtividadeRadiologica atividade)
        {
            if (atividade == null)
            {
                throw new ArgumentNullException(nameof(atividade), "A atividade não pode ser nula.");
            }

            AtividadeRadiologica.Validar(atividade.Nome, atividade.DoseMicrosievert, atividade.FrequenciaAnual);

            int proximoId = _atividades.Count > 0 ? checked(_atividades.Max(a => a.Id) + 1) : 1;
            var registro = Copiar(atividade);
            registro.Id = proximoId;
            var candidatos = _atividades.Append(registro).ToList();
            Gravar(candidatos);
            _atividades.Add(registro);
            atividade.Id = proximoId;
        }

        public void Salvar() => Gravar(_atividades);

        private void Gravar(List<AtividadeRadiologica> dados)
        {
            string temporario = _caminhoArquivo + "." + Guid.NewGuid().ToString("N") + ".tmp";
            try
            {
                ValidarDados(dados);
                string json = JsonSerializer.Serialize(dados, _jsonOptions);
                using (var stream = new FileStream(temporario, FileMode.CreateNew, FileAccess.Write, FileShare.None))
                {
                    byte[] bytes = System.Text.Encoding.UTF8.GetBytes(json);
                    stream.Write(bytes);
                    stream.Flush(flushToDisk: true);
                }
                // Mesmo diretório/volume: nunca truncar o arquivo de destino.
                if (File.Exists(_caminhoArquivo))
                    File.Replace(temporario, _caminhoArquivo, _caminhoArquivo + ".bak");
                else
                    File.Move(temporario, _caminhoArquivo);
            }
            catch (Exception ex)
            {
                throw new InvalidOperationException($"Falha ao salvar '{_caminhoArquivo}'. Cadastro não confirmado: {ex.Message}", ex);
            }
            finally
            {
                // Uma falha de limpeza não pode mascarar a falha original de gravação.
                try { if (File.Exists(temporario)) File.Delete(temporario); }
                catch (IOException) { }
                catch (UnauthorizedAccessException) { }
            }
        }

        private void CarregarOuInicializar()
        {
            try
            {
                if (File.Exists(_caminhoArquivo))
                {
                    string json = File.ReadAllText(_caminhoArquivo);
                    var dados = JsonSerializer.Deserialize<List<AtividadeRadiologica>>(json, _jsonOptions);
                    if (dados == null) throw new InvalidDataException("O JSON deve conter uma lista, não null.");
                    ValidarDados(dados);
                    _atividades.AddRange(dados);
                    return;
                }
            }
            catch (Exception ex)
            {
                throw new InvalidOperationException($"Não foi possível carregar '{_caminhoArquivo}'. O arquivo foi preservado; revise ou restaure uma cópia antes de tentar novamente. {ex.Message}", ex);
            }

            // Dados iniciais (Seed educativo)
            _atividades.Clear();
            _atividades.AddRange(ObterDadosIniciais());
            Salvar();
        }

        private static AtividadeRadiologica Copiar(AtividadeRadiologica a) =>
            new(a.Id, a.Nome, a.Categoria, a.Descricao, a.DoseMicrosievert, a.FrequenciaAnual);

        private static void ValidarDados(List<AtividadeRadiologica> dados)
        {
            var ids = new HashSet<int>();
            foreach (var a in dados)
            {
                if (a == null || a.Id <= 0 || !ids.Add(a.Id))
                    throw new InvalidDataException("Registro nulo, ID inválido ou duplicado no arquivo.");
                AtividadeRadiologica.Validar(a.Nome, a.DoseMicrosievert, a.FrequenciaAnual);
                if (a.Categoria == null || a.Descricao == null)
                    throw new InvalidDataException("Categoria e descrição não podem ser null.");
            }
        }

        private static List<AtividadeRadiologica> ObterDadosIniciais()
        {
            // Doses médicas típicas da tabela FDA; voo é hipótese didática explícita.
            // Não regrava automaticamente catálogos existentes. Veja docs/fontes-e-limitacoes.md.
            return new List<AtividadeRadiologica>
            {
                new(1, "Raio-X de Tórax PA", "Radiologia diagnóstica",
                    "Valor típico ilustrativo da FDA para incidência PA; não representa qualquer radiografia nem a dose individual.", 20.0, 1),
                new(2, "Raio-X de Coluna Lombar", "Radiologia diagnóstica",
                    "Valor típico ilustrativo da tabela FDA. A dose depende do protocolo e do paciente.", 1500.0, 1),
                new(3, "Voo hipotético de 10 horas", "Radiação cósmica em voo",
                    "Exemplo aritmético: 10 horas x 5 µSv/h. Parâmetro didático, não estimativa de uma rota real.", 50.0, 1),
                new(4, "Tomografia Computadorizada de Abdômen", "Radiologia diagnóstica",
                    "Valor típico ilustrativo da FDA; não é violação de limite de exposição do público nem avaliação clínica.", 8000.0, 1),
                new(5, "Tomografia Computadorizada de Crânio", "Radiologia diagnóstica",
                    "Valor típico ilustrativo da tabela FDA. Não equivale a avaliação médica individual.", 2000.0, 1)
            };
        }
    }
}
