using System;
using System.Text.Json.Serialization;

namespace NexoNuclear.Models
{
    /// <summary>
    /// Entidade que representa uma atividade ou evento com emissão de radiação ionizante
    /// no cotidiano, medicina, indústria ou geração de energia.
    /// </summary>
    public class AtividadeRadiologica
    {
        public int Id { get; set; }
        public string Nome { get; set; } = string.Empty;
        public string Categoria { get; set; } = string.Empty;
        public string Descricao { get; set; } = string.Empty;

        /// <summary>
        /// Estimativa de dose efetiva por evento em microsieverts (µSv); não dose absorvida em gray.
        /// </summary>
        public double DoseMicrosievert { get; set; }

        /// <summary>
        /// Frequência estimada da atividade no período de 1 ano.
        /// </summary>
        public int FrequenciaAnual { get; set; }

        /// <summary>
        /// Dose total acumulada no ano em microsieverts (µSv).
        /// </summary>
        [JsonIgnore]
        public double DoseTotalAnual => DoseMicrosievert * FrequenciaAnual;

        public AtividadeRadiologica() { }

        public AtividadeRadiologica(int id, string nome, string categoria, string descricao, double doseMicrosievert, int frequenciaAnual)
        {
            Validar(nome, doseMicrosievert, frequenciaAnual);
            Id = id;
            Nome = nome;
            Categoria = categoria;
            Descricao = descricao;
            DoseMicrosievert = doseMicrosievert;
            FrequenciaAnual = frequenciaAnual;
        }

        /// <summary>
        /// Validações dos atributos conforme regras de domínio.
        /// Lança ArgumentException se algum dado for inválido.
        /// </summary>
        public static void Validar(string nome, double doseMicrosievert, int frequenciaAnual)
        {
            if (string.IsNullOrWhiteSpace(nome) || nome.Trim().Length < 3)
            {
                throw new ArgumentException("O nome da atividade deve possuir no mínimo 3 caracteres.");
            }

            if (!double.IsFinite(doseMicrosievert) || doseMicrosievert <= 0 ||
                !double.IsFinite(doseMicrosievert * frequenciaAnual))
            {
                throw new ArgumentException("A dose deve ser finita, maior que zero e produzir um total anual finito.");
            }

            if (frequenciaAnual <= 0)
            {
                throw new ArgumentException("A frequência anual deve ser de pelo menos 1 ocorrência.");
            }
        }
    }
}
