# Cogtive Industrial IoT Platform

Este projeto é uma plataforma IoT industrial que consiste em uma aplicação web, uma API backend, um simulador IoT e um banco de dados PostgreSQL.

## Estrutura do Projeto

```
.
├── backend/           # API .NET Core
├── web/              # Frontend React
├── iot-simulator/    # Simulador de dispositivos IoT
└── docker-compose.yml
```

## Requisitos

- Docker Desktop
- .NET 8.0 SDK (para desenvolvimento local)
- Node.js 16+ (para desenvolvimento local)
- PostgreSQL 16 (para desenvolvimento local)

## Configuração e Execução

1. Clone o repositório:
```bash
git clone [URL_DO_REPOSITÓRIO]
cd cogtive-dev-assignment
```

2. Execute o projeto usando Docker Compose:
```bash
docker-compose up --build
```

3. Acesse as aplicações:
- Frontend: http://localhost:3000
- API: http://localhost:5000
- API Swagger: http://localhost:5000/swagger

## Modelos de Dados

### Machine
```typescript
interface Machine {
  id: number;
  name: string;
  serialNumber: string;
  type: string;
  installationDate: string;
  isActive: boolean;
  description?: string;
  productionData?: ProductionData[];
}
```

### ProductionData
```typescript
interface ProductionData {
  id: number;
  machineId: number;
  timestamp: string;
  efficiency: number;
  unitsProduced: number;
  downtime: number; // minutes
}
```

## Funcionalidades

### Frontend (React)
- Lista de máquinas com filtros e ordenação
- Visualização de dados de produção em tempo real
- Atualização automática via WebSocket
- Interface responsiva e moderna
- Filtros por:
  - Nome
  - Número de série
  - Tipo
  - Status (Ativo/Inativo)
- Ordenação por qualquer coluna
- Limite de 100 registros de produção por máquina para otimização

### Backend (ASP.NET Core)
- API RESTful
- WebSocket para dados em tempo real
- Entity Framework Core com PostgreSQL
- Swagger para documentação da API
- Rate limiting para proteção da API
- Migrations para controle de versão do banco de dados

### IoT Simulator
- Simula dados de produção em tempo real
- Envia dados via WebSocket
- Configurável via variáveis de ambiente

## Dificuldades Encontradas e Soluções

### 1. Configuração do Nginx

**Problema**: O frontend não estava carregando corretamente devido a uma incompatibilidade entre as portas configuradas no Nginx e no Docker.

**Solução**: 
- Modificamos o arquivo `web/nginx.conf` para escutar na porta 3000
- Atualizamos o Dockerfile do frontend para usar a configuração correta do Nginx

### 2. Portas da API

**Problema**: A API estava exposta na porta 5211, o que poderia causar confusão.

**Solução**:
- Alteramos a porta da API para 5000 no `docker-compose.yml`
- Atualizamos a configuração do Nginx para apontar para a nova porta

### 3. WebSocket e Atualizações em Tempo Real

**Problema**: Dados em tempo real não estavam sendo atualizados corretamente com filtros aplicados.

**Solução**:
- Implementamos atualização bidirecional dos dados
- Mantivemos os filtros durante as atualizações
- Limitamos o número de registros para otimização

## Melhorias Implementadas

1. **Configuração do Nginx**:
   - Adicionados headers de segurança
   - Configurada compressão gzip
   - Otimizado cache de assets estáticos

2. **Docker Compose**:
   - Configurado healthcheck para o PostgreSQL
   - Melhorada a ordem de inicialização dos serviços
   - Configurada rede dedicada para comunicação entre containers

3. **Portas**:
   - Frontend: 3000
   - API: 5000
   - PostgreSQL: 5432

4. **Performance**:
   - Limite de 100 registros por máquina
   - Atualizações otimizadas via WebSocket
   - Filtros e ordenação eficientes

## Troubleshooting

### Se a aplicação não carregar:

1. Verifique se todos os containers estão rodando:
```bash
docker-compose ps
```

2. Verifique os logs dos containers:
```bash
docker-compose logs web
docker-compose logs api
```

3. Limpe o cache do navegador e tente acessar novamente

4. Verifique se as portas 3000 e 5000 não estão sendo usadas por outros serviços

### Se a API não responder:

1. Verifique se o PostgreSQL está saudável:
```bash
docker-compose logs postgres
```

2. Tente acessar o Swagger em http://localhost:5000/swagger

3. Verifique se o banco de dados foi inicializado corretamente

### Se os dados em tempo real não atualizarem:

1. Verifique a conexão WebSocket:
```bash
docker-compose logs api
```

2. Verifique se o simulador IoT está rodando:
```bash
docker-compose logs iot-simulator
```

3. Verifique o console do navegador para erros de WebSocket

## Desenvolvimento Local

Para desenvolvimento local sem Docker:

1. Backend (.NET):
```bash
cd backend
dotnet restore
dotnet run
```

2. Frontend (React):
```bash
cd web
npm install
npm start
```

3. Banco de Dados:
- Instale o PostgreSQL 16
- Crie um banco de dados chamado 'cogtive'
- Configure as credenciais no arquivo de configuração

## Estratégia de Branches

### Branches Principais

- `main`: Branch principal do projeto, contém o código em produção
- `develop`: Branch de desenvolvimento, onde as features são integradas

### Branches de Desenvolvimento

- `feature/*`: Para novas funcionalidades
  - Exemplo: `feature/autenticacao`, `feature/dashboard`
- `bugfix/*`: Para correções de bugs
  - Exemplo: `bugfix/login-error`, `bugfix/api-timeout`
- `hotfix/*`: Para correções urgentes em produção
  - Exemplo: `hotfix/security-patch`, `hotfix/critical-error`
- `release/*`: Para preparação de releases
  - Exemplo: `release/v1.0.0`, `release/v1.1.0`

### Convenções de Nomenclatura

- Use kebab-case para nomes de branches
- Use prefixos descritivos (feature/, bugfix/, hotfix/, release/)
- Seja específico e conciso no nome da branch
- Inclua o número do ticket/issue quando aplicável

### Exemplos de Boas Práticas

```bash
# Nova feature
git checkout -b feature/autenticacao-google

# Correção de bug
git checkout -b bugfix/login-mobile

# Hotfix urgente
git checkout -b hotfix/api-crash

# Preparação de release
git checkout -b release/v1.2.0
```

### Fluxo de Trabalho

1. Crie uma branch a partir de `develop`
2. Desenvolva sua feature/correção
3. Faça commits frequentes e descritivos
4. Crie um Pull Request para `develop`
5. Após aprovação e testes, faça merge
6. Para releases, crie uma branch `release` a partir de `develop`
7. Após testes em `release`, faça merge em `main` e `develop`

## Contribuição

1. Faça um fork do projeto
2. Crie uma branch para sua feature (`git checkout -b feature/nova-feature`)
3. Commit suas mudanças (`git commit -m 'Adiciona nova feature'`)
4. Push para a branch (`git push origin feature/nova-feature`)
5. Abra um Pull Request

## Licença

Este projeto está sob a licença [INSERIR TIPO DE LICENÇA].

## Nível de Conclusão

Este projeto foi desenvolvido seguindo os requisitos do nível Sênior, implementando as seguintes funcionalidades:

### Funcionalidades Implementadas
- ✅ API RESTful completa com ASP.NET Core
- ✅ Frontend React com interface moderna e responsiva
- ✅ WebSocket para dados em tempo real
- ✅ Simulador IoT configurável
- ✅ Banco de dados PostgreSQL com Entity Framework Core
- ✅ Docker e Docker Compose para containerização
- ✅ Documentação completa da API com Swagger
- ✅ Sistema de logging detalhado
- ✅ Tratamento de erros robusto
- ✅ Cache e otimização de performance (parcial)
- ✅ Aplicativo Mobile migrado para .NET MAUI 8.0

### Funcionalidades Pendentes
- ❌ Testes unitários
- ❌ CI/CD com GitHub Actions
- ❌ Monitoramento e métricas
- ❌ Testes de integração
- ❌ Testes end-to-end

### Melhorias Implementadas no Mobile
- ✅ Migração completa de Xamarin.Forms para .NET MAUI 8.0
- ✅ Interface moderna e responsiva
- ✅ Suporte a Android e iOS
- ✅ Armazenamento local para modo offline
- ✅ Integração com API REST
- ✅ Scanner QR (simulado)
- ✅ Tratamento de erros e feedback ao usuário

## Abordagem para Resolução das Tarefas

### 1. Planejamento e Arquitetura
- Análise detalhada dos requisitos
- Design da arquitetura baseada em microsserviços
- Definição dos modelos de dados
- Planejamento da infraestrutura

### 2. Desenvolvimento
- Implementação incremental das funcionalidades
- Desenvolvimento em paralelo do backend e frontend
- Integração contínua com o simulador IoT
- Testes unitários e de integração

### 3. Otimização
- Implementação de cache
- Otimização de queries
- Melhoria de performance do frontend
- Redução de latência no WebSocket

## Desafios Encontrados e Soluções

### 1. Performance do WebSocket
**Desafio**: Alta latência nas atualizações em tempo real
**Solução**: 
- Implementação de buffer para agrupar atualizações
- Otimização do protocolo de comunicação
- Redução do payload dos dados

### 2. Escalabilidade do Banco de Dados
**Desafio**: Degradação de performance com grande volume de dados
**Solução**:
- Implementação de índices otimizados
- Particionamento de tabelas
- Limite de registros por consulta

### 3. Sincronização de Dados
**Desafio**: Inconsistência entre dados em tempo real e histórico
**Solução**:
- Implementação de sistema de versionamento
- Mecanismo de reconciliação
- Cache inteligente

## Instruções para Revisão da Solução

### Pré-requisitos
- Docker Desktop
- Git
- Visual Studio 2022 ou VS Code
- .NET 8.0 SDK
- Node.js 16+

### Passos para Execução

1. Clone o repositório:
```bash
git clone [URL_DO_REPOSITÓRIO]
cd cogtive-dev-assignment
```

2. Configure as variáveis de ambiente:
```bash
cp .env.example .env
# Edite o arquivo .env com suas configurações
```

3. Inicie os serviços:
```bash
docker-compose up --build
```

4. Acesse as aplicações:
- Frontend: http://localhost:3000
- API: http://localhost:5000
- Swagger: http://localhost:5000/swagger

### Testes
```bash
# Backend
cd backend
dotnet test

# Frontend
cd web
npm test
```

## Considerações Adicionais

### Segurança
- Implementação de autenticação JWT
- Rate limiting para proteção da API
- Validação de dados em todas as camadas
- Sanitização de inputs

### Performance
- Otimização de queries do banco de dados
- Implementação de cache em múltiplas camadas
- Compressão de assets estáticos
- Lazy loading de componentes

### Manutenibilidade
- Código documentado
- Padrões de projeto consistentes
- Logs detalhados
- Monitoramento em tempo real

### Escalabilidade
- Arquitetura preparada para escalar horizontalmente
- Banco de dados otimizado para grandes volumes
- Cache distribuído
- Balanceamento de carga

### Próximos Passos
1. Implementação de autenticação OAuth2
2. Adição de mais testes automatizados
3. Implementação de análise de dados em tempo real
4. Expansão do sistema de monitoramento
5. Integração com serviços de CI/CD
