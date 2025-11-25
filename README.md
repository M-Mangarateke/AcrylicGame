# Acrylic Game

Branch-aware beauty studio booking site built with ASP.NET Core MVC, Identity, and Entity Framework Core. Clients can book services and upload proof of payment, staff manage branch-level content, and admins see high-level stats.

## Features
- Identity with roles (`Admin`, `Staff`, `Client`); registration assigns `Client` automatically, seeded roles and accounts on startup, and a console-based dummy email sender for notifications.
- Branch-aware data: branches are seeded (Durban, Rivonia, Sunninghill, Parkhurst) and tie together bookings, promotions, gallery items, and testimonials.
- Bookings: clients submit bookings with a 50% deposit proof upload (JPG/PNG/PDF), view status in a dashboard, and staff can confirm or reject bookings for their branch.
- Promotions & gallery: staff create time-bound promotions with images and upload gallery items with captions/credits; public pages can be filtered by branch.
- Testimonials: clients submit testimonials; staff approve or delete branch-specific submissions; approved testimonials are shown publicly with optional branch filter.
- Dashboards & content: admin sees totals for users/bookings/promotions; staff dashboard lists branch bookings and pending testimonials; public pages include services (PDF downloads), gallery, promotions, and contact form that stores messages.

## Tech Stack
- .NET 9, ASP.NET Core MVC + Identity UI
- Entity Framework Core (SQL Server by default)
- Bootstrap 5, Bootstrap Icons, custom CSS under `wwwroot/css/site.css`
- File uploads saved under `wwwroot/uploads` (payments, promotions, gallery)

## Project Structure
- `AcrylicGame/Controllers`, `Models`, `Views`, `Data/ApplicationDbContext.cs` – MVC + EF Core setup
- `Areas/Identity` – default Identity UI with a customized register flow that assigns the `Client` role
- `wwwroot/` – static assets, PDF service sheets in `downloads/`, uploaded media in `uploads/`
- `Program.cs` – DI, middleware, and seeding for roles, users, and branches

## Prerequisites
- .NET SDK 9.x
- SQL Server (LocalDB or full SQL Server). EF migrations target SQL Server; adjust connection string as needed.
- (Optional) Docker if you prefer containerized runs.

## Running Locally
```powershell
cd AcrylicGame
dotnet run
```
- Launch settings expose `https://localhost:7065` and `http://localhost:5226` by default.
- Authenticated flows: register/login via Identity UI; use seeded staff/admin accounts for management features.

## Docker
```powershell
cd AcrylicGame
docker build -t acrylicgame .
docker run -e "ConnectionStrings__DefaultConnection=Server=...;Database=...;User Id=...;Password=...;TrustServerCertificate=True" -p 8080:80 acrylicgame
```
Ensure the connection string points to a reachable SQL Server instance from inside the container.

## File Uploads & Static Assets
- Proof of payment uploads → `wwwroot/uploads/payments`
- Promotion images → `wwwroot/uploads/promotions`
- Gallery photos → `wwwroot/uploads/gallery`
- Service PDFs → `wwwroot/downloads`
These folders are created automatically on upload; make sure the app has write permissions in your hosting environment.

## Useful Commands
- Create a migration: `dotnet ef migrations add <Name>`
- Update database: `dotnet ef database update`
- Run with hot reload: `dotnet watch run`

## Notes
- Email sending uses `DummyEmailSender` (writes to console). Swap in a real `IEmailSender` implementation and configure SMTP/API keys for production.
- Password policy is relaxed for demo purposes (no digits/uppercase required, length ≥ 6); tighten it for production deployments.
