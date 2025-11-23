# Form Service Application

## Technologies

**Backend:**
- .NET 10
- Entity Framework Core 10.0.0
- SQLite with FTS5 (Full-Text Search)
- Swagger & Scalar (API Documentation)

**Frontend:**
- Vue.js 3.5
- TypeScript 5.9
- Vue Router 4.6
- Vite 7.2

## Build Instructions

### Backend

```bash
cd IFormService
dotnet restore
dotnet build
```

### Frontend

```bash
cd UIForm/UIForm
npm install
npm run build
```

### Run Locally

**Backend:**
```bash
cd IFormService/IFormService.Api
dotnet run
```

**Frontend:**
```bash
cd UIForm/UIForm
npm run dev
```

### Test

**Backend:**
```bash
cd IFormService
dotnet test
```
