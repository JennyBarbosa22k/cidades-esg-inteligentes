# Projeto - Cidades ESGInteligentes

API REST (Java 17 + Spring Boot 3) para cadastro e consulta de indicadores ESG de cidades
(índice ESG, emissão de CO₂ e percentual de área verde), com pipeline CI/CD completo,
containerização com Docker e deploy automatizado em **staging** e **produção**.

**Integrantes:** _preencher nome e RM de cada integrante_

## Como executar localmente com Docker

Pré-requisitos: Docker e Docker Compose.

```bash
git clone <url-do-repositorio>
cd cidades-esg-inteligentes
cp .env.example .env          # ajuste a senha do banco
docker compose up -d --build  # sobe app + PostgreSQL
```

Testes rápidos:

```bash
curl http://localhost:8080/actuator/health
curl -X POST http://localhost:8080/api/cidades -H "Content-Type: application/json" \
  -d '{"nome":"Curitiba","estado":"PR","indiceEsg":82,"emissaoCo2":2.1,"areaVerdePercentual":64.5}'
curl http://localhost:8080/api/cidades
```

Para derrubar: `docker compose down` (acrescente `-v` para apagar o volume do banco).

### Endpoints

| Método | Rota | Descrição |
|---|---|---|
| GET | /api/cidades | Lista cidades |
| GET | /api/cidades/{id} | Busca por id |
| POST | /api/cidades | Cria cidade |
| PUT | /api/cidades/{id} | Atualiza cidade |
| DELETE | /api/cidades/{id} | Remove cidade |
| GET | /actuator/health | Health check |

## Pipeline CI/CD

**Ferramenta:** GitHub Actions (`.github/workflows/ci-cd.yml`). Imagens publicadas no GitHub Container Registry (GHCR).

| Etapa (job) | O que faz | Quando roda |
|---|---|---|
| Build e Testes | `mvn clean package` e `mvn test` (4 testes automatizados com MockMvc + H2) | push e pull request na `main` |
| Build e Push Docker | Gera a imagem e publica no GHCR com tag do commit (`sha`) e `latest` | push na `main` |
| Deploy Staging | Acessa o servidor por SSH, executa `deploy/deploy.sh`, faz smoke test em `/actuator/health` | após a imagem |
| Deploy Produção | Mesmo processo para produção, **com aprovação manual** (Required reviewers) | após staging OK |

Se qualquer etapa falhar, as seguintes não são executadas. Staging e produção usam a **mesma imagem** (tag do commit), garantindo que o que foi testado é o que vai ao ar.

### Configuração necessária no GitHub

1. **Settings > Environments**: criar `staging` e `production` (em `production`, ativar *Required reviewers*).
2. Em cada environment, cadastrar os **secrets** `SSH_HOST`, `SSH_USER`, `SSH_KEY` e as **variables** `STAGING_URL` / `PRODUCTION_URL`
   (ex.: `http://IP:8081` e `http://IP:8080`).
3. Settings > Actions > General > Workflow permissions: *Read and write*.
4. Tornar o pacote GHCR público ou executar `docker login ghcr.io` no servidor.

### Preparação do servidor (uma vez)

Uma VM com Docker (ex.: Azure VM, AWS EC2, Oracle Cloud free tier), portas 8080 e 8081 liberadas:

```bash
mkdir -p ~/esg/staging ~/esg/production
# copie docker-compose.yml para as duas pastas e crie um .env em cada uma:
# staging:    COMPOSE_PROJECT_NAME=esg-staging     APP_PORT=8081  POSTGRES_PASSWORD=...
# production: COMPOSE_PROJECT_NAME=esg-production  APP_PORT=8080  POSTGRES_PASSWORD=...
```

O `COMPOSE_PROJECT_NAME` isola redes, containers e volumes de cada ambiente.

## Containerização

**Dockerfile** (multi-stage):

```dockerfile
# ---------- Estagio 1: build ----------
FROM maven:3.9-eclipse-temurin-17 AS build
WORKDIR /app
COPY pom.xml .
RUN mvn -B -q dependency:go-offline
COPY src ./src
RUN mvn -B -q clean package -DskipTests

# ---------- Estagio 2: runtime (imagem enxuta) ----------
FROM eclipse-temurin:17-jre-alpine
WORKDIR /app
RUN addgroup -S esg && adduser -S esg -G esg
COPY --from=build /app/target/cidades-esg-*.jar app.jar
USER esg
EXPOSE 8080
HEALTHCHECK --interval=30s --timeout=5s --start-period=40s --retries=3 \
  CMD wget -qO- http://localhost:8080/actuator/health || exit 1
ENTRYPOINT ["java", "-jar", "app.jar"]
```

Estratégias adotadas:

- **Multi-stage build**: o Maven fica só no estágio de build; a imagem final usa apenas JRE Alpine (menor e mais segura).
- **Cache de dependências**: `pom.xml` é copiado antes do código para reaproveitar camadas.
- **Usuário não-root** e **HEALTHCHECK** via Actuator.
- **docker-compose.yml**: serviços `app` + `db` (PostgreSQL 16), **volume** `pgdata` (persistência), **rede** `esg-net`,
  **variáveis de ambiente** vindas do `.env`, `depends_on` com `service_healthy` para o app só subir após o banco estar pronto.

## Prints do funcionamento

> Insira as imagens em `docs/prints/` e referencie abaixo.

| Evidência | Arquivo |
|---|---|
| Pipeline executando (build + testes) | ![pipeline](docs/prints/01-pipeline.png) |
| Testes passando | ![testes](docs/prints/02-testes.png) |
| Deploy em staging | ![staging](docs/prints/03-deploy-staging.png) |
| Aprovação e deploy em produção | ![producao](docs/prints/04-deploy-producao.png) |
| Staging funcionando (/actuator/health e /api/cidades) | ![staging-app](docs/prints/05-staging-funcionando.png) |
| Produção funcionando | ![prod-app](docs/prints/06-producao-funcionando.png) |

Link do repositório / Actions: _preencher_

## Tecnologias utilizadas

Java 17, Spring Boot 3.3 (Web, Data JPA, Validation, Actuator), PostgreSQL 16, H2 (testes), Maven,
JUnit 5 + MockMvc, Docker, Docker Compose, GitHub Actions, GitHub Container Registry, SSH.

## Checklist de entrega

| Item | OK |
|---|---|
| Projeto compactado em .ZIP com estrutura organizada | ☑ |
| Dockerfile funcional | ☑ |
| docker-compose.yml ou arquivos Kubernetes | ☑ |
| Pipeline com etapas de build, teste e deploy | ☑ |
| README.md com instruções e prints | ☐ (inserir prints) |
| Documentação técnica com evidências (PDF ou PPT) | ☐ (inserir prints) |
| Deploy realizado nos ambientes staging e produção | ☐ (executar o pipeline) |
