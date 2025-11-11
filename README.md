# 🏍️ Vogel Rentals API

API desenvolvida em **.NET 8 / C#** para o desafio técnico de backend.  
Ela simula um sistema de **gestão de motos, entregadores e locações**, incluindo regras de negócio, persistência em PostgreSQL e arquitetura modular.

---

## 🧩 Visão geral

A API segue uma arquitetura em camadas:

src/ \
├── Vogel.Rentals.Api             → Ponto de entrada da aplicação (controllers) \
├── Vogel.Rentals.Application     → Casos de uso, serviços e validadores \
├── Vogel.Rentals.Domain          → Entidades e regras de domínio \
├── Vogel.Rentals.Infrastructure  → Persistência (EF Core / Postgres) e integrações externas

---

## ⚙️ Como Rodar o Projeto

### 🔸 Pré-requisitos

- [Docker Desktop](https://www.docker.com/products/docker-desktop) instalado e em execução  
- Opcional: [Postman](https://www.postman.com/) ou [Insomnia](https://insomnia.rest/) para testar os endpoints (As collections estão disponíveis na pasta `collections/`)

---
### 🔹 Passos para execução

1. **Clone o repositório**

   ```bash
   git clone <seu-repo-ou-fork>
   ```

<br>

2. **Suba os containers com Docker Compose**

   No terminal, execute:

   ```bash
   docker-compose up --build
   ```

    Isso iniciará: \
        - A API: http://localhost:8080 \
        - O banco PostgreSQL: localhost:5432 \

<br>

3. **Acesse a documentação Swagger**

- http://localhost:8080/swagger \
Nele estão todos os endpoints disponíveis para teste.

🗃️ Banco de Dados

PostgreSQL é utilizado como base de dados.
Os dados de conexão padrão (definidos no docker-compose.yml) são:

| Variável | Valor         |
| -------- | ------------- |
| Host     | localhost     |
| Porta    | 5432          |
| Database | vogel_rentals |
| Usuário  | vogel         |
| Senha    | vogel123      |

<br>

🧱 Tecnologias Utilizadas \
	•	.NET 8 / C# \
	•	Entity Framework Core \
	•	PostgreSQL \
	•	Docker + Docker Compose \
	•	Swagger (Swashbuckle) \
	•	Clean Architecture / Repository Pattern \

<br>

🧠 Decisões Técnicas \
	•	Arquitetura limpa: divisão entre Domain, Application, Infrastructure e Api para desacoplamento e testabilidade. \
	•	Repositories: abstraem o acesso a dados, permitindo trocar o armazenamento (InMemory → Postgres). \
	•	Services: concentram regras de negócio, mantendo Controllers simples. \
	•	Validators: centralizam validações de entrada. \
	•	Tratamento de exceções: via middlewares customizados e BusinessRuleException para erros de negócio.

<br>

👨‍💻 Autor

Desenvolvido por [Leonardo Vogel](https://www.linkedin.com/in/leonardovogel/)  \
Contato: [contato@leovogel.dev](contato@leovogel.dev)
