# Fontes, hipóteses e limites da PoC

## Decisão educativa

Esta é uma simulação aritmética para aprendizagem, não um dosímetro, diagnóstico,
avaliação de risco individual ou certificação de conformidade. Categorias livres
cadastradas pelo usuário não permitem inferir enquadramento regulatório.

A revisão remove a classificação "segura", o alerta de acompanhamento e a porcentagem
do limite civil. Também remove equivalências alimentares como indicador de segurança
e a soma com uma média global de fundo natural, que não é uma medição da pessoa.
Separar contextos é uma decisão de escopo desta PoC, não a afirmação de que doses
nunca possam ser agregadas numa avaliação técnica apropriada.

## Referência regulatória

Norma ANSN 3.01, Requisitos Básicos de Radioproteção e Segurança Radiológica de Fontes
de Radiação, Resolução CNEN 344/25. Portal oficial:
https://www.gov.br/cnen/pt-br/acesso-rapido/normas/grupo-3

Texto da norma:
https://www.gov.br/cnen/pt-br/acesso-rapido/normas/grupo-3/ansn-3-01-requisitos-basicos-de-radioprotecao-e-seguranca-radiologica-de-fontes-de-radiacao.pdf

- Art. 6º, XLI: a definição de exposição do público exclui exposições ocupacional e
  médica e a radiação de fundo natural normal do local.
- Art. 4º: exclui do escopo, entre outros casos especificados no artigo, a exposição
  devida à presença de potássio-40 no corpo humano.

Essas disposições não significam ausência de risco ou dispensa de radioproteção na
medicina. A PoC simplesmente não aplica o limite de exposição do público aos exames,
ao consumo alimentar ou à soma indiscriminada desses cenários. Também não avalia
exposição ocupacional de tripulantes. Não fornece orientação jurídica ou clínica.

## Valores dos exemplos médicos

FDA, "What are the Radiation Risks from CT?", tabela 1:
https://www.fda.gov/radiation-emitting-products/medical-x-ray-imaging/what-are-radiation-risks-ct

| Procedimento da tabela | Dose efetiva típica (mSv) | Parâmetro em µSv |
| --- | ---: | ---: |
| Radiografia de tórax, incidência PA | 0,02 | 20 |
| Radiografia de coluna lombar | 1,5 | 1500 |
| Tomografia de abdômen | 8 | 8000 |
| Tomografia de crânio | 2 | 2000 |

A FDA ressalta a variação por paciente, equipamento e protocolo. Valores típicos não
podem ser associados com precisão a um exame individual. A quantidade de radiografias
do simulador é especificamente de tórax PA; a de tomografias é de abdômen, não de
qualquer região. Radiografias e tomografias são radiologia diagnóstica.
O simulador não recomenda, contraindica ou altera exames. Dúvidas clínicas devem ser
tratadas com a equipe de saúde responsável.

## Hipóteses explicitamente didáticas

- Voo: 5 µSv/h é um parâmetro fixo ilustrativo adotado pela PoC, não um valor universal
  nem uma dose validada para a rota São Paulo–Paris. O exemplo do catálogo é apenas
  um cenário hipotético de dez horas; não uma medição de voo comercial real.
- Alimentação: contagem semanal multiplicada por 52 semanas ilustrativas. Não há
  conversão em dose acumulada, equivalência de bananas ou inferência de segurança.
- Fundo natural: nenhum valor médio é somado ao resultado da simulação.
- Catálogo: dose por evento multiplicada por frequência é uma estimativa aritmética
  cadastrada, não cálculo de risco. O usuário deve documentar a hipótese na descrição.
- O antigo exemplo quantitativo de proximidade de Angra foi retirado dos novos seeds:
  não havia fundamento verificado para aquele número, local ou comparação com o PNM.
- Catálogos antigos são preservados, não migrados silenciosamente. Sua revisão é manual.

## Referência acadêmica

Documento fornecido pelo usuário: `2TDS Agosto - Marinha 2026 - 1º Semestre V2 (1).pdf`,
seções Advanced Business Development with .NET 1/3–3/3. O documento pede console com
entidade, validações, regra, cadastro, consultas e persistência; não exige que esta
PoC realize classificação regulatória ou forneça recomendação médica.
