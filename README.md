# 🚀 AuraScale - Gestão de Escalas Inteligente

O **AuraScale** é uma aplicação Full Stack desenvolvida para automatizar e organizar escalas de trabalho de equipes e gestores. O projeto foi construído com foco em escalabilidade, utilizando as tecnologias mais recentes do ecossistema .NET e infraestrutura em nuvem no Microsoft Azure.

## 🌐 Link do Projeto
O sistema está online e pode ser acessado em:  
https://aurascale.azurewebsites.net

## 🛠️ Tecnologias Utilizadas

### **Back-end**
* **C# / .NET 10**: Linguagem e framework principal da aplicação.
* **ASP.NET Core MVC**: Arquitetura do sistema.
* **Entity Framework Core**: ORM para comunicação com o banco de dados.
* **ASP.NET Core Identity**: Sistema de autenticação e autorização.
* **Google OAuth 2.0**: Integração para login social seguro.

### **Banco de Dados**
* **Azure SQL Database**: Banco de dados relacional em nuvem.

### **Front-end**
* **Bootstrap 5 (Tema Quartz)**: Design responsivo e moderno.
* **FontAwesome**: Ícones para melhor experiência do usuário.
* **JavaScript / jQuery**: Interações dinâmicas e componentes UI.

### **Infraestrutura & DevOps**
* **Microsoft Azure**: Hospedagem via App Service.
* **GitHub Actions**: Pipeline de CI/CD para deploy automatizado.

## 📋 Funcionalidades
- [x] **Autenticação Segura**: Login via Google e sistema de contas nativo.
- [x] Portal do Colaborador: Consulta individual de escala personalizada via login por e-mail.
- [x] **Gestão de Operadores**: Cadastro completo da equipe.
- [x] **Importação de Dados**: Suporte para carga massiva de operadores via arquivo XLSX (Excel).
- [x] Inteligência de Dados: Validação automática de conflitos de horário para impedir duplicidade de escalas.
- [x] **Dashboard Responsivo**: Interface adaptada para desktop e dispositivos móveis.

## 💻 Como Rodar o Projeto Localmente

### 1. Pré-requisitos
* .NET 10 SDK instalado.
* SQL Server (LocalDB ou Express).
* Visual Studio ou IDE de sua preferência .

### 2. Clonar o Repositório
```bash
git clone https://github.com/nathatargino/AuraScale.git
cd AuraScale
```

## 3. Configurar o Banco de Dados
* Abra o arquivo appsettings.json e ajuste a Connection String para o seu SQL Server local:
```bash
"ConnectionStrings": {
  "DefaultConnection": "Server=(localdb)\\mssqllocaldb;Database=AuraScale;Trusted_Connection=True;MultipleActiveResultSets=true"
}
```

## 4. Executar Migrations
No Gerenciador de Pacotes ou Terminal, execute os comandos para criar as tabelas:
```bash
dotnet ef database update
```

## 5. Iniciar a Aplicação
```bash
dotnet run
```

## 👤 Desenvolvedor
**Nathã Targino** Desenvolvedor FullStack.

- LinkedIn: https://www.linkedin.com/in/nathatargino/
- GitHub: https://github.com/nathatargino
