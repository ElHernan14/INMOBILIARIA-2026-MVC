# Nombre del Proyecto

> Sistema de gestión inmobiliaria desarrollado con ASP.NET MVC, Entity Framework y MySQL Server.

---

## 👥 Integrantes del Grupo

- **Hernán Gonzalo Constante** - *hernanbonne98@gmail.com* - [@usuario_github](https://github.com/ElHernan14) - Discord: `usuario_discord`
- **Nicanor Arancibia** - *nikanor_95@hotmail.com* - [@usuario_github](https://github.com/Nicanor95) - Discord: `usuario_discord`
- **Kevin Paredes** - *kevinenriquep26@gmail.com* - [@usuario_github](https://github.com/kevinpa26) - Discord: `usuario_discord`

---

## 📐 Modelado de Datos

A continuación se presenta el esquema del modelo de datos correspondiente a la aplicación:

### Diagrama Entidad-Relación (DER) / Diagrama de Clases

# ![Diagrama del Proyecto](./docs/diagram/diagrama.png)

# INMOBILIARIA-2026-MVC

Sistema de gestión inmobiliaria desarrollado con ASP.NET MVC, Entity Framework y MySQL Server.

LINK DEL DIAGRAMA: https://lucid.app/lucidchart/71855755-75b8-46e5-8ac2-116c8883de4f/edit?beaconFlowId=961B8247B06847BB&invitationId=inv_0eefedc3-e21a-4d97-ab79-5cdab212a256&page=0_0#

### INSTRUCCIONES:

# 📌 Proyecto INMOBILIARIA – .NET + MySQL/TiDB

## 🚀 Requisitos previos

- .NET 6 SDK [(dotnet.microsoft.com in Bing)](https://www.bing.com/search?q="https%3A%2F%2Fdotnet.microsoft.com%2Fen-us%2Fdownload%2Fdotnet%2F6.0") o superior
- [Visual Studio Code](https://code.visualstudio.com/) con extensión **C# Dev Kit**
- MySQL/TiDB instalado o acceso a un cluster remoto

---

## ⚙️ Configuración de conexión

En `appsettings.json`:

```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Server=localhost;Port=3306;Database=inmobiliaria;User Id=root;Password=TU_PASSWORD;SslMode=None;"
  }
}
```

- Cambiá `Server`, `Port`, `Database`, `User Id` y `Password` según tu entorno.

---

## 🗄️ Setup de base de datos (SQL inicial)

- Se encuentra en el script en la raíz: ./db_inmobiliaria_lab_2.sql

---

## ▶️ Ejecución del proyecto

1. Restaurar dependencias:
   ```bash
   dotnet restore
   ```
2. Compilar en modo debug:
   ```bash
   dotnet build --configuration Debug
   ```
3. Ejecutar:
   ```bash
   dotnet run
   ```
4. La API se levanta en:
   ```
   https://localhost:5001
   http://localhost:5000
   ```

---

## 🐞 Debugging en VS Code

1. Abrí el proyecto en VS Code.
2. Poné breakpoints en tus controllers/repositorios.
3. Usá **F5** o `dotnet run` para iniciar en modo debug.
4. El flujo se detiene en los breakpoints y podés inspeccionar variables.

---

## 📡 Endpoints disponibles POSTMAN

1. Se encuentran en el archivo compartido en la raíz: ./INMO LAB2 2026.postman_collection.json
2. Luego este archivo se usa para importar como colección en POSTMAN.

---

```

```
