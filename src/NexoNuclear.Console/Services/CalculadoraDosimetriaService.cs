using System;
using NexoNuclear.Models;

namespace NexoNuclear.Services
{
    /// <summary>
    /// Serviço que implementa as regras de negócio de dosimetria, comparação educativa
    /// e classificação de segurança com base nas normas da CNEN (Comissão Nacional de Energia Nuclear)
    /// e IAEA (Agência Internacional de Energia Atômica).
    /// </summary>
    public class CalculadoraDosimetriaService
    {
        /// <summary>
        /// Limite anual de dose efetiva para indivíduos do público civil segundo a Norma CNEN-NN-3.01:
        /// 1 mSv/ano = 1.000 µSv/ano.
        /// </summary>
        public const double LimitePublicoAnualMicrosievert = 1000.0;

        /// <summary>
        /// Dose média de radiação natural de fundo ingerida ao comer 1 banana rica em Potássio-40: ~0.1 µSv.
        /// </summary>
        public const double DoseEquivalenteBanana = 0.1;

        /// <summary>
        /// Dose cósmica média recebida por hora em voo comercial de cruzeiro: ~5.0 µSv/hora.
        /// </summary>
        public const double DoseHoraVooComercial = 5.0;

        /// <summary>
        /// Regra de Negócio: Avalia e classifica o nível de exposição acumulada anual.
        /// </summary>
        public string ClassificarRiscoAnual(double doseTotalAnual)
        {
            if (doseTotalAnual <= 50.0)
            {
                return "Insignificante (Equivalente ao nível de fundo natural diário)";
            }
            if (doseTotalAnual <= 500.0)
            {
                return "Baixa / Segura (Típica de exames simples ou viagens aéreas eventuais)";
            }
            if (doseTotalAnual <= LimitePublicoAnualMicrosievert)
            {
                return "Moderada (100% dentro do limite regulatório anual da CNEN para o público)";
            }

            return "Atenção: Acima da recomendação anual civil da CNEN (Requer acompanhamento radiológico)";
        }

        /// <summary>
        /// Regra de Negócio: Retorna a porcentagem do limite seguro da CNEN que a atividade representa.
        /// </summary>
        public double CalcularPorcentagemLimiteCnen(double doseTotalAnual)
        {
            return Math.Round((doseTotalAnual / LimitePublicoAnualMicrosievert) * 100.0, 2);
        }

        /// <summary>
        /// Regra de Negócio: Calcula equivalência em bananas (BED - Banana Equivalent Dose).
        /// </summary>
        public double CalcularEquivalenciaBananas(double doseTotalAnual)
        {
            return Math.Round(doseTotalAnual / DoseEquivalenteBanana, 1);
        }

        /// <summary>
        /// Regra de Negócio: Calcula equivalência em horas de voo comercial.
        /// </summary>
        public double CalcularEquivalenciaHorasVoo(double doseTotalAnual)
        {
            return Math.Round(doseTotalAnual / DoseHoraVooComercial, 1);
        }
    }
}
