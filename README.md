# NexoNuclear — PoC educativa .NET

FIAP — Sprint 1 (Sprint Flash), Advanced Business Development with .NET.
Challenge 2026: Marinha do Brasil / AMAZUL / CTMSP. Grupo Nexo.

## Integrantes

| Nome Completo | RM | Turma | Responsabilidade Principal |
| :--- | :--- | :--- | :--- |
| **Alisson Kawan Evangelista Silva** | **RM567598** | 2TDS | Responsável Técnico (Java e .NET) |
| **Kenji** | **RM568156** | 2TDS | IoT e Banco de Dados |
| **Erick Ramos** | **RM567837** | 2TDS | DevOps Tools & Cloud Computing |
| **Eduardo Boni** | **RM567236** | 2TDS | Mobile Application Development |
| **Marcos Vinicius de Jesus Almeida** | **RM567214** | 2TDS | Documentação e Qualidade de Software (QA) |

A identificação acima não comprova contribuição no Git. A participação real de todos
continua pendente de conferência e contribuição individual; veja
[verificação e pendências](docs/verificacao-e-pendencias.md).

## Solução e escopo

O Nexo propõe aproximar a sociedade das aplicações e benefícios da tecnologia nuclear.
Este repositório é somente o entregável acadêmico .NET: uma aplicação console com
cadastro, listagem, busca por nome/categoria/descrição e persistência em JSON.
Não é o produto completo, não é uma API e não depende do backend Java ou de Oracle.

## Como o .NET se aplica à solução do grupo?

O .NET demonstra cálculos educativos por contexto: estimativas de exames de radiologia
diagnóstica e um cenário hipotético de voos. Isso complementa o catálogo de aplicações
nucleares da API Java, sem repetir sua funcionalidade. As regras são a validação do
cadastro, dose por evento multiplicada pela frequência anual e a simulação separada
por contexto. Não há classificação de segurança, diagnóstico, porcentagem de limite
regulatório ou recomendação de exames. Esta PoC não certifica instalações ou atividades
da Marinha/AMAZUL.

## O que foi desenvolvido

- Console C# com menu contínuo, tratamento de erros e encerramento em fim de entrada.
- Entidade `AtividadeRadiologica`: ID, nome, categoria, descrição, estimativa de dose
  efetiva por evento (µSv), frequência e resultado aritmético anual.
- Nome com ao menos três caracteres; dose positiva e finita; frequência positiva;
  rejeição de NaN, infinitos e overflow do resultado anual.
- Simulador: exames médicos e voos exibidos separadamente. Alimentação aparece apenas
  como contagem ilustrativa, sem conversão de bananas em dose acumulada.
- Cinco exemplos iniciais com valores médicos típicos da FDA ou hipótese de voo
  explicitamente didática. Fontes e hipóteses em [fontes e limitações](docs/fontes-e-limitacoes.md).
- JSON validado na leitura, gravação em temporário no mesmo diretório e substituição
  do destino com backup anterior. Cadastro só confirmado após gravação bem-sucedida.
- Testes de regressão executáveis, incluindo subprocessos reais do console.

## Arquitetura

- `src/NexoNuclear.Console/Models/AtividadeRadiologica.cs`: entidade e validações.
- `src/NexoNuclear.Console/Services/CalculadoraDosimetriaService.cs`: cálculos educativos
  e `ResultadoSimulacao`, sem classificação regulatória.
- `src/NexoNuclear.Console/Services/RepositorioAtividadesJson.cs`: cadastro, consultas,
  validação e persistência. Consultas devolvem cópias para evitar alteração interna acidental.
- `src/NexoNuclear.Console/Program.cs`: interação, menu e tratamento de erros.
- `tests/NexoNuclear.Regression`: executor de regressão sem pacotes externos.
- `docs/`: checklist derivado, fontes e registro de verificação/pendências.

## Pré-requisito e execução

Instale o .NET SDK 10.0.x. O alvo é `net10.0`: apenas o SDK 8 não compila este projeto.
Confira a instalação com `dotnet --list-sdks`.

Na raiz deste repositório:

```bash
dotnet build src/NexoNuclear.Console --configuration Release --warnaserror
dotnet run --project src/NexoNuclear.Console
```

No Visual Studio, abra `src/NexoNuclear.Console/NexoNuclear.Console.csproj`.
A pasta avulsa `NexoNuclear.Console` fora deste repositório não é mantida automaticamente.

Menu: 1 listar; 2 buscar; 3 cadastrar; 4 simular; 5 contexto educativo; 0 sair.
No cadastro, use ponto ou vírgula decimal, por exemplo `0.5` ou `0,5`.

## Dados, backup e recuperação

O arquivo `atividades_nucleares.json` fica ao lado do executável, por exemplo
`src/NexoNuclear.Console/bin/Debug/net10.0/` no `dotnet run` padrão. Release usa outro
diretório: Debug e Release não compartilham dados automaticamente. Faça cópia dos
arquivos antes de limpar `bin`, trocar configuração ou substituir a instalação.

- Os exemplos iniciais são criados somente se o arquivo de dados não existir.
- Um catálogo válido `[]` continua vazio; não é repovoado automaticamente.
- JSON corrompido, registro inválido ou falha de leitura impede a abertura com uma
  mensagem explicativa e saída controlada. O arquivo existente não é sobrescrito.
- Ao substituir um arquivo existente, `.bak` guarda a versão anterior. É um backup
  rotativo, não um histórico completo; a gravação seguinte pode substituí-lo.
- Para recuperar: feche o console, copie o arquivo problemático para um local seguro,
  confira o `.bak` e só então restaure uma cópia válida no caminho original. Não
  remova o original para tentar recuperar registros: isso apenas criaria exemplos novos.
- Dados de versões antigas não são migrados nem corrigidos automaticamente. Revise
  descrições e hipóteses antigas, especialmente bananas, Angra e classificações médicas.
  Para demonstrar o catálogo novo, use uma instalação separada sem arquivo de dados,
  preservando o catálogo antigo e seus cadastros.
- Escopo: uma instância local por arquivo. Não abrir duas instâncias nem editar o
  JSON enquanto o console está aberto. Não há coordenação de escritores concorrentes.
- Substituição segura depende do suporte do sistema de arquivos. Falha é reportada,
  sem fallback destrutivo. Isso não é garantia contra toda falha de hardware.
- Não cadastrar dados pessoais ou históricos médicos reais. Os exemplos são didáticos.

## Testes

```bash
dotnet run --project tests/NexoNuclear.Regression --configuration Release
```

É um executor próprio, não uma suíte xUnit/MSTest: execute com `dotnet run`, não use
`dotnet test` como prova de aprovação. O processo retorna zero quando todos passam e
um em caso de falha. Usa diretórios isolados e não altera seu catálogo de uso.
Detalhes: [tests/README.md](tests/README.md).

## Limitações e entrega

Os valores não representam a dose individual e não devem orientar decisões de saúde.
Não somamos contextos distintos para compará-los ao limite de exposição do público.
Veja [fontes e limitações](docs/fontes-e-limitacoes.md) e o
[checklist derivado da rubrica](docs/requisitos-sprint-1.md).
A aprovação técnica não substitui revisão humana, participação real dos integrantes
ou conferência da submissão no portal FIAP. Não há garantia de nota ou de atendimento integral.
