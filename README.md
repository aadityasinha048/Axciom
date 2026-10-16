# AcxiomCRM

AcxiomCRM is an enterprise Customer Relationship Management (CRM) web application built using **ASP.NET Core MVC (.NET 8)**, **Entity Framework Core 8**, **SQLite**, and **Bootstrap 5**.

---

## Features (Milestone 1: Customer Management)

The Customer Management module implements full CRUD workflows and search capabilities aligned with the assignment specification:

### 1. Mandatory Customer Fields
- **CustomerId**: Integer, auto-incremented primary key.
- **CustomerCode**: Unique sequential code (e.g. `CUST-00001`) with automatic collision resolution and case normalization.
- **CustomerName**: Mandatory, max 100 characters.
- **Email**: Mandatory, valid email, max 150 characters with database-level uniqueness and whitespace/case normalization.
- **Phone**: Mandatory, 10-digit Indian mobile format (`^[6-9]\d{9}$`) with database-level uniqueness and whitespace normalization.
- **CompanyName**: Optional, max 150 characters.
- **Address**: Mandatory street address, max 250 characters.
- **City**: Mandatory city name, max 100 characters.
- **State**: Mandatory state name, max 100 characters.
- **Status**: Mandatory customer status (`Active`, `Prospect`, `Lead`, `Inactive`).
- **CreatedDate**: Automatically set on the server as `DateTime.UtcNow`.
- **CreatedBy**: Automatically populated from `User.Identity.Name` or explicitly set to `"System"` in development.

### 2. Functional Capabilities
- **Customer Listing & Multi-Field Search**: Search across Customer Code, Name, Email, Phone, Company, City, State, or Status.
- **Create Customer**: Server-side model validation, automatic sequential code generation, and duplicate checking.
- **Customer Details**: Categorized card layout (Profile, Contact Channels, Address, and Audit history).
- **Edit Customer**: Full updates with validation, protecting unique constraints while preserving audit creation fields.
- **Delete Customer**: Safety warning prompt with complete summary card before confirmed deletion.
- **Security & Integrity**: Protected against CSRF attacks with `[ValidateAntiForgeryToken]` and using parameterized EF Core LINQ queries.

---

## Tech Stack
- **Framework**: ASP.NET Core 8.0 MVC (C#)
- **ORM**: Entity Framework Core 8.0.10
- **Database**: SQLite
- **Frontend / Styling**: Bootstrap 5, HTML5, Vanilla CSS

---

## Getting Started

### Prerequisites
- [.NET 8.0 SDK](https://dotnet.microsoft.com/download/dotnet/8.0)
- EF Core CLI Tool:
  ```powershell
  dotnet tool install --global dotnet-ef
  ```

### Setup & Run Instructions

1. **Clone the repository**:
   ```bash
   git clone https://github.com/aadityasinha048/Axciom.git
   cd Axciom
   ```

2. **Restore dependencies**:
   ```bash
   dotnet restore
   ```

3. **Apply database migrations**:
   ```bash
   dotnet ef database update
   ```

4. **Run the application**:
   ```bash
   dotnet run --launch-profile http
   ```

5. **Access the application**:
   - Home: `http://localhost:5227`
   - Customer Directory: `http://localhost:5227/Customers`

---

## Project Structure

```text
AcxiomCRM/
│
├── Controllers/
│   ├── CustomersController.cs    # Customer CRUD, search, duplicate checks, audit logic
│   └── HomeController.cs         # Default landing and privacy pages
│
├── Data/
│   └── ApplicationDbContext.cs   # EF Core DbContext, entity mapping, unique indexes
│
├── Migrations/                   # EF Core schema migration history
│
├── Models/
│   ├── Customer.cs               # Customer entity with data annotations and auto-trimming
│   └── ErrorViewModel.cs         # Error view model
│
├── Views/
│   ├── Customers/
│   │   ├── Index.cshtml          # Customer directory, search bar, status badges
│   │   ├── Create.cshtml         # Customer creation form
│   │   ├── Edit.cshtml           # Customer editing form
│   │   ├── Details.cshtml        # Complete 12-field customer profile
│   │   └── Delete.cshtml         # Deletion confirmation page
│   ├── Home/
│   └── Shared/
│       └── _Layout.cshtml        # Navigation header with Customers menu
│
├── appsettings.json              # Connection string & app configuration
└── Program.cs                    # Application startup, DI services, SQLite registration
```
