# NexoNuclear — Prova de Conceito .NET de Dosimetria e Educação Nuclear

**FIAP — Sprint 1 (Sprint Flash) — Advanced Business Development with .NET**  
**Challenge 2026:** Parceria Marinha do Brasil / AMAZUL (Centro Tecnológico da Marinha em São Paulo - CTMSP)  
**Grupo:** Nexo  

---

## 1. Integrantes do Grupo (Nexo)

| Nome Completo | RM | Turma | Responsabilidade Principal |
| :--- | :--- | :--- | :--- |
| **Alisson Kawan Evangelista Silva** | **RM567598** | 2TDS | Responsável Técnico (Java e .NET) |
| **Kenji** | **RM568156** | 2TDS | IoT e Banco de Dados |
| **Erick Ramos** | **RM567837** | 2TDS | DevOps Tools & Cloud Computing |
| **Eduardo Boni** | **RM567236** | 2TDS | Mobile Application Development |
| **Marcos Vinicius de Jesus Almeida** | **RM567214** | 2TDS | Documentação e Qualidade de Software (QA) |

---

## 2. Descrição da Solução

O **Nexo** é uma solução digital educativa criada para aproximar a sociedade da tecnologia nuclear, desmistificando concepções errôneas sobre radiação e energia nuclear, e demonstrando suas aplicações reais em saúde, indústria, agricultura, ciência e geração de energia.

Este projeto em **.NET** constitui a Prova de Conceito (PoC) do módulo analítico e educativo de dosimetria cotidiana. Ele calcula a exposição a radiações ionizantes de fundo natural e atividades humanas (alimentos, exames médicos, voos comerciais) e compara os valores acumulados com as diretrizes da **Norma CNEN-NN-3.01** e da **IAEA** (*International Atomic Energy Agency*).

---
## 3. Como o .NET se aplica à solução do grupo?

> *"Dentro do ecossistema da solução educacional Nexo, o .NET atua como o módulo analítico e de simulação interativa de dosimetria. Enquanto a API em Java centraliza o catálogo institucional das aplicações e impactos sociais da tecnologia nuclear, o componente em .NET é responsável pelo processamento de cálculos radiológicos e comparação de risco com base em dados técnicos. Ele traduz doses de radiação (microSieverts) em equivalências cotidianas e compreensíveis para o público leigo (como doses equivalentes de bananas ou horas de voo), reforçando a missão de educar a sociedade e evidenciar os protocolos de excelência e segurança adotados pela Marinha do Brasil e pela AMAZUL no PNM."*

---

## 4. O que foi desenvolvido nesta Sprint

- **Aplicação Console Interativa em C# (.NET 10.0 / LTS):** Interface via terminal com menus organizados e navegação contínua.
- **Entidade de Domínio (`AtividadeRadiologica`):**
  - Atributos: `Id`, `Nome`, `Categoria`, `Descricao`, `DoseMicrosievert`, `FrequenciaAnual`, `DoseTotalAnual`.
  - Validações: nome mínimo de 3 caracteres, dose estritamente maior que zero, frequência maior que zero.
- **Regras de Negócio e Dosimetria (`CalculadoraDosimetriaService`):**
  - Classificação de risco em conformidade com o limite anual para indivíduos do público da CNEN (1.000 µSv/ano = 1 mSv/ano).
  - Porcentagem de consumo do limite regulatório anual.
  - Equivalência educativa: BED (*Banana Equivalent Dose*, 0,1 µSv) e horas de voo comercial (5,0 µSv/h).
- **Simulador Interativo Pessoal:** O usuário responde sobre hábitos (exames de Raio-X, tomografias, voos e consumo alimentar) e obtém o total anual estimado comparado à radiação natural de fundo do planeta (2.400 µSv/ano).
- **Persistência de Dados em Arquivo JSON (`RepositorioAtividadesJson`):**
  - Arquivo `atividades_nucleares.json` com persistência automática de novos cadastros.
  - Carga inicial (*seed*) com 5 atividades reais cobrindo medicina, indústria, transporte aéreo e o Programa Nuclear da Marinha.
- **Tratamento Global de Exceções:** Blocos `try/catch` para impedir encerramentos abruptos em entradas inválidas.

Consulte o [checklist oficial de requisitos](docs/requisitos-sprint-1.md) para detalhes de conformidade.

---

## 5. Estrutura do Código

```text
s1-dotnet-poc/
├── docs/
│   └── requisitos-sprint-1.md                 # Checklist de conformidade da Sprint 1
├── src/
│   └── NexoNuclear.Console/
│       ├── Models/
│       │   └── AtividadeRadiologica.cs        # Entidade de domínio e validações
│       ├── Services/
│       │   ├── CalculadoraDosimetriaService.cs # Regras de dosimetria, CNEN e equivalências
│       │   └── RepositorioAtividadesJson.cs    # Persistência em JSON e sementes
│       ├── Program.cs                         # Ponto de entrada e menu interativo
│       └── NexoNuclear.Console.csproj         # Configuração do projeto .NET 10
├── .gitignore                                 # Regras de exclusão Git
└── README.md                                  # Documentação principal
```

---

## 6. Instruções de Execução e Teste

### Pré-requisitos:
- [.NET 8.0 SDK ou superior / .NET 10.0](https://dotnet.microsoft.com/download)

### Comandos para Compilar e Executar:
```bash
# 1. Navegue até a pasta do repositório
cd src/NexoNuclear.Console

# 2. Compile a aplicação
dotnet build

# 3. Execute o console
dotnet run
```

Ou a partir da raiz do repositório:
```bash
dotnet run --project src/NexoNuclear.Console
```
