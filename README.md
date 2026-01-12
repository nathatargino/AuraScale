# ⚖️ AuraScale - Gestão Inteligente de Escalas

O **AuraScale** é uma plataforma robusta desenvolvida para automatizar a criação e gestão de escalas de trabalho. O foco principal do projeto é facilitar a vida de gestores através de uma interface moderna e um motor de processamento inteligente de dados.

## 🚀 Funcionalidades Principais

* **Dashboard de Controle**: Visão geral da operação em tempo real com interface otimizada.
* **Importação Inteligente via Excel**: Processamento de grandes volumes de dados de operadores utilizando a biblioteca **ClosedXML**, com lógica automática para criação de novos modelos de escala caso não existam no sistema.
* **Gestão de Operadores**: CRUD completo com visualização de detalhes em formato de perfil, facilitando a revisão de vínculos de escala.
* **Modelos Dinâmicos**: Criação de regras de carga horária para dias úteis, sábados e domingos.
* **Autenticação Segura**: Login integrado via **Google OAuth 2.0** com proteção de credenciais através de **User Secrets**.

## 🛠️ Tecnologias Utilizadas

* **Framework**: ASP.NET Core 8.0 (MVC).
* **Linguagem**: C# (utilizando recursos avançados de LINQ, Expression Lambdas e Interfaces).
* **Persistência**: Entity Framework Core com SQL Server (ou SQLite para desenvolvimento).
* **Processamento de Arquivos**: ClosedXML (Leitura e Escrita de .xlsx).
* **Frontend**: Bootstrap 5 com componentes customizados (Dark Theme/Modern UI).

## 🔒 Segurança e Melhores Práticas

Como o foco do projeto é o desenvolvimento Back-End de alta confiabilidade, foram aplicadas as seguintes camadas de proteção:

* **Gerenciamento de Segredos**: Utilização do **Secret Manager (User Secrets)** do .NET para armazenar chaves de API e Client Secrets em ambiente de desenvolvimento, impedindo que dados sensíveis sejam rastreados pelo controle de versão.
* **Prevenção de Vazamentos (Git)**: Arquivo `.gitignore` rigorosamente configurado para excluir pastas de build (`bin/`, `obj/`), caches da IDE (`.vs/`) e arquivos de configuração que possam conter strings de conexão locais.
* **Autenticação e Identidade**: Integração com **Google OAuth 2.0** utilizando o middleware de autenticação do ASP.NET Core. O sistema não apenas valida a identidade do usuário, mas também gerencia o ciclo de vida dos claims, permitindo que cada gerente possua um ambiente isolado e seguro para seus dados.
* **Proteção de Push**: Configuração de regras de proteção no GitHub para identificar e bloquear automaticamente o upload de segredos expostos no `appsettings.json`.

## 🏁 Como Rodar o Projeto

1. Clone o repositório:
   ```bash
   git clone [https://github.com/nathatargino/AuraScale.git](https://github.com/nathatargino/AuraScale.git)
