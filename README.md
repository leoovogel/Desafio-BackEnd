# 🏍️ Vogel Rentals API

**API REST para aluguel de motos e gestão de entregadores**, desenvolvida em **.NET 8 (C#)**, seguindo Clean Architecture, DDD e boas práticas.  
Principais tecnologias: ASP.NET Core, PostgreSQL, Docker, RabbitMQ, Amazon S3 (com fallback local), FluentValidation, xUnit, Serilog e Swagger.

---

### ⚙️ Requisitos

- **Docker** *(caso queira rodar com docker)* ou - **.NET SDK 8.0+** *(caso queira rodar localmente)*
- **AWS Account** *(opcional, apenas se quiser testar S3 real)*

## ▶️ Como Rodar o Projeto (Docker / local)

> **Observação:** O armazenamento de imagens de CNH pode ser feito via **Amazon S3** (se variáveis AWS estiverem configuradas) ou, caso contrário, será utilizado **armazenamento local** como fallback automático.

### 1️⃣ Clonar o repositório
```bash
git clone https://github.com/leoovogel/Desafio-BackEnd.git
cd Desafio-BackEnd
```

### 2️⃣ Subir os containers
Execute na raiz do projeto:
```bash
docker-compose up --build
```

Isso irá subir os serviços:
| Serviço    | Porta   | Descrição                      |
|------------|---------|--------------------------------|
| API        | `8080`  | API REST para aluguel de motos |
| PostgreSQL | `5432`  | Banco de dados                 |
| RabbitMQ   | `15672` | Painel de mensageria           |

---

### 3️⃣ Acessar a aplicação

##### ✅ Health Check:
```bash
curl http://localhost:8080/hc
```

Retorno esperado:

```json
{ "status": "ok" }
```

<br>

##### 🌐 Swagger:
Abra no navegador:

[http://localhost:8080/swagger](http://localhost:8080/swagger)

---

### 4️⃣ Variáveis de Ambiente (opcional)
As variáveis padrão já estão no docker-compose.yml, então o projeto funciona imediatamente.
Mas você pode renomear o arquivo `.env.example` para `.env` e usar suas próprias keys aws se quiser testar o S3 real.

**🔹 Banco de dados**
```yaml
ConnectionStrings__DefaultConnection=Host=db;Port=5432;Database=vogel_rentals;Username=vogel;Password=vogel123
```

**🔹 RabbitMQ**
```yaml
RabbitMq__HostName=vogel-rabbitmq
RabbitMq__UserName=guest
RabbitMq__Password=guest
RabbitMq__QueueName=motorcycle_created
```

**🔹 Amazon S3 (opcional)**
Caso deseje testar o upload real para um bucket AWS:
```yaml
export AWS_ACCESS_KEY_ID=SEU_ACCESS_KEY
export AWS_SECRET_ACCESS_KEY=SEU_SECRET
export S3Storage__BucketName=seu-bucket
export S3Storage__Region=usa-east-1
```
> Se essas variáveis não estiverem presentes, a API automaticamente usa armazenamento local em disco (sem dependências externas).

---

### 5️⃣ Banco de dados e migrations

O banco é criado automaticamente via Docker.

Conexão para acessar via DataGrip/DBeaver:
	•	Host: localhost
	•	Porta: 5432
	•	Banco: vogel_rentals
	•	Usuário: vogel
	•	Senha: vogel123

---

### 6️⃣ RabbitMQ
Acesse o painel de controle do RabbitMQ em:
> [http://localhost:15672](http://localhost:15672)

Login:
- Usuário: `guest`
- Senha: `guest`

Quando uma moto é cadastrada, a aplicação publica uma mensagem nessa fila.
Se o ano da moto for 2024, o consumidor grava uma notificação no banco.

---

### 7️⃣ Testes Unitários

Para rodar os testes unitários, execute:
```bash
dotnet test
```

---

## 📖 Detalhes do Projeto

### 🚀 Tecnologias Utilizadas

- **.NET 8 / ASP.NET Core Web API**
- **Entity Framework Core** (PostgreSQL)
- **Docker & Docker Compose**
- **RabbitMQ** (mensageria e eventos assíncronos)
- **Amazon S3** *(com fallback para armazenamento local)*
- **FluentValidation** (validações customizadas)
- **xUnit + FluentAssertions + Moq** (testes unitários)
- **Serilog / Microsoft Logging** (logs estruturados)
- **Swagger** (documentação automática de endpoints)

---

### 🏗️ Estrutura de Pastas

src/ \
 ├── Vogel.Rentals.Api/             → Controllers, Middlewares, Startup \
 ├── Vogel.Rentals.Application/     → Services, Interfaces, Validators \
 ├── Vogel.Rentals.Domain/          → Entidades e Regras de Negócio \
 ├── Vogel.Rentals.Infrastructure/  → EF, Repositórios, Context, S3, RabbitMQ \
tests/ \
 └── Vogel.Rentals.Tests.Unit/      → Testes unitários

---

### 👨‍💻 Autor

[Leonardo Vogel](https://www.linkedin.com/in/leonardovogel/) \
Desenvolvedor Backend .NET \
📧 [contato@leovogel.dev](contato@leovogel.dev) \

