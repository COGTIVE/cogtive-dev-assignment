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
- .NET 7.0 SDK (para desenvolvimento local)
- Node.js 16+ (para desenvolvimento local)
- PostgreSQL 14 (para desenvolvimento local)

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

### 3. Favicon

**Problema**: Erro 404 ao tentar carregar o favicon.ico.

**Solução**:
- O erro é apenas um aviso e não afeta a funcionalidade da aplicação
- Pode ser resolvido adicionando um favicon ao projeto no futuro

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
- Instale o PostgreSQL 14
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
