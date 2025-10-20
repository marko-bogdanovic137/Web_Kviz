# QuizApp

## Opis
Web aplikacija za polaganje kvizova, praćenje rezultata i rang liste. 
Frontend: Angular 17, Backend: .NET 8, baza: SQL Server (LocalDB).

## Pokretanje

### Backend
1. Otvoriti projekat u Visual Studio 2022
2. Pokrenuti bazu (LocalDB ili SQL Server)
3. Build i Run backend (`F5` ili `Ctrl+F5`)
4. API će biti dostupan na: `https://localhost:7108/api`

### Frontend
1. Otvoriti terminal u folderu `frontend/`
2. Instalirati dependencije: `npm install`
3. Pokrenuti aplikaciju: `ng serve`
4. Aplikacija je dostupna na `http://localhost:4200/`

## Arhitektura
- Frontend: Angular, stand-alone komponente, servisi za HTTP pozive
- Backend: .NET 8, MVC/WebAPI, tro-slojna arhitektura (Controller -> Service -> DbContext)
- JWT autentifikacija, heširane lozinke
- DTO-ovi za komunikaciju front-backend

## Funkcionalnosti
- Registracija / Login
- Pregled kvizova i filtriranje
- Rešavanje kviza sa vremenskim limitom
- Pregled ličnih i globalnih rezultata
- Admin CRUD nad kvizovima i pitanjima
