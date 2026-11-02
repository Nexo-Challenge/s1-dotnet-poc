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
            return _atividades.OrderBy(a => a.Id).ToList().AsReadOnly();
        }

        public AtividadeRadiologica? BuscarPorId(int id)
        {
            return _atividades.FirstOrDefault(a => a.Id == id);
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
                .ToList();
        }

        public void Cadastrar(AtividadeRadiologica atividade)
        {
            if (atividade == null)
            {
                throw new ArgumentNullException(nameof(atividade), "A atividade não pode ser nula.");
            }

            AtividadeRadiologica.Validar(atividade.Nome, atividade.DoseMicrosievert, atividade.FrequenciaAnual);

            int proximoId = _atividades.Count > 0 ? _atividades.Max(a => a.Id) + 1 : 1;
            atividade.Id = proximoId;

            _atividades.Add(atividade);
            Salvar();
        }

        public void Salvar()
        {
            try
            {
                string json = JsonSerializer.Serialize(_atividades, _jsonOptions);
                File.WriteAllText(_caminhoArquivo, json);
            }
            catch (Exception ex)
            {
                throw new InvalidOperationException($"Falha ao salvar o arquivo de dados em '{_caminhoArquivo}': {ex.Message}", ex);
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
                    if (dados != null && dados.Count > 0)
                    {
                        _atividades.Clear();
                        _atividades.AddRange(dados);
                        return;
                    }
                }
            }
            catch (Exception ex)
            {
                // Log silencioso ou fallback para não quebrar a aplicação
                System.Console.WriteLine($"[Aviso] Não foi possível ler o arquivo existente ({ex.Message}). Inicializando catálogo padrão...");
            }

            // Dados iniciais (Seed educativo)
            _atividades.Clear();
            _atividades.AddRange(ObterDadosIniciais());
            Salvar();
        }

        private static List<AtividadeRadiologica> ObterDadosIniciais()
        {
            return new List<AtividadeRadiologica>
            {
                new(
                    1,
                    "Comer 1 Banana (Potássio-40)",
                    "Alimentação Cotidiana",
                    "Alimentos ricos em potássio possuem isótopo radioativo natural K-40 perfeitamente seguro ao corpo humano.",
                    0.1,
                    365
                ),
                new(
                    2,
                    "Raio-X de Tórax",
                    "Medicina Nuclear / Diagnóstico",
                    "Exame médico de imagem fundamental para detecção precoce de doenças respiratórias.",
                    20.0,
                    1
                ),
                new(
                    3,
                    "Voo Comercial São Paulo - Paris (Ida e Volta)",
                    "Radiação Cósmica",
                    "Em altitudes de cruzeiro (10.000m), a atmosfera mais rarefeita absorve menos raios cósmicos solares.",
                    50.0,
                    1
                ),
                new(
                    4,
                    "Tomografia Computadorizada de Abdômen",
                    "Medicina Nuclear / Diagnóstico",
                    "Exame complexo de múltiplos cortes diagnósticos com alto benefício clínico de salvamento de vidas.",
                    8000.0,
                    1
                ),
                new(
                    5,
                    "Viver a 10 km da Usina Nuclear Angra 1/2 por 1 Ano",
                    "Geração de Energia / PNM",
                    "Usinas nucleares modernas utilizam contenções robustas; a dose ambiental é menor que a radiação natural do solo de Guarapari/ES.",
                    0.09,
                    1
                )
            };
        }
    }
}
