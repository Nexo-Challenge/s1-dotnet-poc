# Regressões executáveis

Pré-requisito: .NET SDK 10.0.x e comando `dotnet` disponível no PATH.
Execute a partir da raiz do repositório:

```bash
dotnet run --project tests/NexoNuclear.Regression --configuration Release
```

Executor C# próprio, sem pacotes NuGet de testes. Não é xUnit/MSTest; `dotnet test`
não executa estas asserções. Saída: PASS/FAIL, resumo contado em execução e código
zero para aprovação ou um para falha.

## Cobertura

- Nome, dose/frequência, NaN, infinitos e overflow do total anual.
- Preservação de JSON inválido, null, registro nulo ou inválido; catálogo vazio.
- Falha real de gravação mediante bloqueio do destino; sem cadastro fantasma nem
  confirmação de ID. Backup e ausência de temporário após a falha.
- Cadastro, busca por nome/categoria/descrição e recarga em nova instância.
- Isolamento de objetos devolvidos pelas consultas e catálogo inicial revisado.
- Simulador separado por contexto; zero, negativos e contagens grandes.
- Subprocessos reais do console: inicialização com arquivo corrompido, EOF,
  listagem, busca, simulação, cadastro decimal, NaN e opção inválida.

Cada execução usa diretórios exclusivos em TMPDIR, quando disponível, ou na pasta
de saída dos testes. Os subprocessos usam cópias dos binários sem acessar seu JSON
de uso. Os diretórios gerados pelo executor são removidos ao final.

O teste de bloqueio de arquivo foi validado no Windows. Em outro sistema, diferenças
na semântica de compartilhamento/substituição de arquivos podem exigir adaptar essa
injeção de falha; não ignore um FAIL nem o trate como teste aprovado.
Não se simulou queda de energia nem corrupção física do dispositivo.