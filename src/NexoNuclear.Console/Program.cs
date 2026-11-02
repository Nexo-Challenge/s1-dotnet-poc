using System;
using System.Collections.Generic;
using System.Globalization;
using NexoNuclear.Models;
using NexoNuclear.Services;

namespace NexoNuclear
{
    internal class Program
    {
        private static readonly RepositorioAtividadesJson Repositorio = new();
        private static readonly CalculadoraDosimetriaService Calculadora = new();

        static void Main(string[] args)
        {
            Console.OutputEncoding = System.Text.Encoding.UTF8;
            bool executando = true;

            while (executando)
            {
                try
                {
                    ExibirMenuPrincipal();
                    Console.Write("Escolha uma opção: ");
                    string? opcao = Console.ReadLine()?.Trim();

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
            Console.Clear();
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
                Console.Write("Nome da atividade (ex: Ressonância / Exame / Voo): ");
                string? nome = Console.ReadLine();

                Console.Write("Categoria (ex: Medicina Nuclear, Indústria, Alimentos, Cotidiano): ");
                string? categoria = Console.ReadLine();

                Console.Write("Descrição / Justificativa educativa: ");
                string? descricao = Console.ReadLine();

                Console.Write("Dose média por evento em microsieverts (µSv) [ex: 20 ou 0.1]: ");
                string? doseStr = Console.ReadLine()?.Replace(',', '.');
                if (!double.TryParse(doseStr, NumberStyles.Any, CultureInfo.InvariantCulture, out double dose) || dose <= 0)
                {
                    Console.ForegroundColor = ConsoleColor.Red;
                    Console.WriteLine("[Erro] Valor de dose inválido. Deve ser um número decimal maior que zero.");
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
            Console.ForegroundColor = ConsoleColor.Yellow;
            Console.WriteLine("--- SIMULADOR INTERATIVO DE EXPOSIÇÃO RADIOLÓGICA PESSOAL ---");
            Console.ResetColor();
            Console.WriteLine("Responda às perguntas abaixo para calcular sua estimativa de dose anual:");
            Console.WriteLine();

            try
            {
                Console.Write("Quantos exames de Raio-X você realizou no último ano? ");
                int raiox = LerInteiroNaoNegativo();

                Console.Write("Quantas Tomografias Computadorizadas você realizou? ");
                int tomografias = LerInteiroNaoNegativo();

                Console.Write("Quantas horas você passou voando em aviões comerciais no ano? ");
                int horasVoo = LerInteiroNaoNegativo();

                Console.Write("Quantas bananas você consome por semana em média? ");
                int bananasSemana = LerInteiroNaoNegativo();

                // Cálculos baseados em doses médias conhecidas
                double doseRaioX = raiox * 20.0; // 20 µSv cada
                double doseTC = tomografias * 8000.0; // 8.000 µSv cada
                double doseVoo = horasVoo * CalculadoraDosimetriaService.DoseHoraVooComercial; // 5 µSv/h
                double doseBananas = (bananasSemana * 52) * CalculadoraDosimetriaService.DoseEquivalenteBanana; // 0.1 µSv cada
                double doseFundoNaturalSolo = 2400.0; // Média global de radiação cósmica e do solo (2.4 mSv = 2400 µSv)

                double doseTotal = doseRaioX + doseTC + doseVoo + doseBananas + doseFundoNaturalSolo;

                Console.WriteLine();
                Console.ForegroundColor = ConsoleColor.Cyan;
                Console.WriteLine("=================== RESULTADO DA SIMULAÇÃO ===================");
                Console.ResetColor();
                Console.WriteLine($"- Radiação Natural de Fundo (Inerente ao planeta): 2.400,00 µSv/ano");
                Console.WriteLine($"- Doses Médicas (Exames): {(doseRaioX + doseTC):N2} µSv/ano");
                Console.WriteLine($"- Radiação Cósmica (Voos): {doseVoo:N2} µSv/ano");
                Console.WriteLine($"- Alimentação (Bananas K-40): {doseBananas:N2} µSv/ano");
                Console.WriteLine("--------------------------------------------------------------");
                Console.ForegroundColor = ConsoleColor.White;
                Console.WriteLine($"Dose Total Estimada no Ano: {doseTotal:N2} µSv ({doseTotal / 1000.0:N2} mSv)");
                Console.ResetColor();

                double doseAntropica = doseRaioX + doseTC + doseVoo + doseBananas;
                string classificacao = Calculadora.ClassificarRiscoAnual(doseAntropica);
                double pctLimite = Calculadora.CalcularPorcentagemLimiteCnen(doseAntropica);

                Console.WriteLine();
                Console.WriteLine($"Classificação de Segurança (atividades civis): {classificacao}");
                Console.WriteLine($"Impacto sobre a norma civil CNEN (1.000 µSv/ano): {pctLimite}% do limite civil");
                Console.WriteLine("==============================================================");
            }
            catch (Exception ex)
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine($"[Erro no Simulador] {ex.Message}");
                Console.ResetColor();
            }
        }

        private static void ExibirSobreProjetoNuclear()
        {
            Console.ForegroundColor = ConsoleColor.Cyan;
            Console.WriteLine("================================================================================");
            Console.WriteLine("      A VERDADE SOBRE A ENERGIA NUCLEAR - PARCERIA MARINHA DO BRASIL / AMAZUL   ");
            Console.WriteLine("================================================================================");
            Console.ResetColor();
            Console.WriteLine("1. GERAÇÃO LIMPA E SEGURA:");
            Console.WriteLine("   A energia nuclear é uma das fontes de eletricidade mais limpas do planeta,");
            Console.WriteLine("   com emissão de gases do efeito estufa próxima de zero durante a operação.");
            Console.WriteLine();
            Console.WriteLine("2. PROGRAMA NUCLEAR DA MARINHA (PNM / CTMSP):");
            Console.WriteLine("   Desde 1979, o Brasil desenvolve de forma 100% autônoma o domínio do ciclo do");
            Console.WriteLine("   combustível nuclear e a tecnologia para propulsão naval e geração de energia.");
            Console.WriteLine();
            Console.WriteLine("3. MEDICINA NUCLEAR E AGRICULTURA:");
            Console.WriteLine("   Radioisótopos produzidos pelo IPEN/CNEN salvam milhares de vidas anualmente no");
            Console.WriteLine("   tratamento e diagnóstico de câncer (ex: Iodo-131 e Tecnécio-99m) e esterilização.");
            Console.WriteLine();
            Console.WriteLine("4. DOSES E SEGURANÇA:");
            Console.WriteLine("   Viver próximo a uma usina nuclear em operação regular expõe o cidadão a uma dose");
            Console.WriteLine("   anual menor do que comer algumas bananas ou fazer um voo entre estados.");
            Console.WriteLine("================================================================================");
        }

        private static void ExibirCardAtividade(AtividadeRadiologica item)
        {
            string risco = Calculadora.ClassificarRiscoAnual(item.DoseTotalAnual);
            double bananas = Calculadora.CalcularEquivalenciaBananas(item.DoseTotalAnual);
            double pctCnen = Calculadora.CalcularPorcentagemLimiteCnen(item.DoseTotalAnual);

            Console.WriteLine($"[ID #{item.Id}] {item.Nome} ({item.Categoria})");
            Console.WriteLine($"    Descrição: {item.Descricao}");
            Console.WriteLine($"    Dose por evento: {item.DoseMicrosievert:N2} µSv | Frequência/Ano: {item.FrequenciaAnual}x");
            Console.WriteLine($"    Dose Total Anual: {item.DoseTotalAnual:N2} µSv ({pctCnen}% do limite anual civil CNEN)");
            Console.WriteLine($"    Classificação: {risco}");
            Console.WriteLine($"    Equivalência Educativa: ~{bananas:N0} bananas consumidas no ano");
            Console.WriteLine(new string('-', 80));
        }

        private static int LerInteiroNaoNegativo()
        {
            while (true)
            {
                string? entrada = Console.ReadLine();
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
