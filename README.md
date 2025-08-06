# 🚌 Route Buddy – Bus Booking Platform  

Route Buddy is a **monolithic web-based bus booking platform** designed to simplify online bus reservations. The platform provides a seamless experience for users to search, view, and book buses with real-time availability and a clean, responsive UI.

---

## 🚀 Features

### 🎯 Core Functionalities:
- 🔎 **Dynamic Bus Search** – Search buses by source, destination, and travel date.
- 🖥️ **Responsive UI** – Optimized for desktop, tablet, and mobile users.
- 🏷️ **Booking Module** – Reserve seats with instant booking confirmation.
- 👤 **User Authentication** – Sign-up, login, and profile management.
- 🚌 **Bus Listings Page** – Filter and view buses with timing, price, and available seats.
- 📜 **Ticket Management** – View booked tickets and travel history.

### 🛠 Planned Enhancements:
- 💳 **Payment Gateway Integration** (Razorpay/Stripe for secure payments).
- 📱 **Progressive Web App (PWA)** for mobile-like experience.
- 🔔 **Email/SMS Notifications** for booking confirmation & reminders.
- ⭐ **Ratings & Reviews** for buses/operators.
- 🔐 **Admin Dashboard** for managing buses, routes, and users.

---

## 🏗️ Tech Stack

### Frontend:
- **HTML5**, **CSS3**, **JavaScript (ES6+)**
- **Bootstrap 5** (Responsive UI components)
  
### Backend:
- **C# (.NET Core MVC)** – Monolithic architecture (simpler for initial build)
- **Entity Framework Core** – ORM for database management

### Database:
- **SQL Server** – Relational database for buses, bookings, and users.

### Tools & Workflow:
- **Git & GitHub** – Version control & collaboration.
- **Docker (Planned)** – Containerized deployment.
- **CI/CD (Planned)** – Automated build & deployment pipeline.

---

## ⚙️ Installation & Setup

### 1️⃣ Clone Repository:
```bash
git clone https://github.com/<your-username>/route-buddy.git
cd route-buddy
2️⃣ Setup Backend:
Open the project in Visual Studio or VS Code.

Configure appsettings.json with your SQL Server connection string.

Run EF Core migrations to create database:

bash
Copy
Edit
dotnet ef database update
3️⃣ Run Application:
bash
Copy
Edit
dotnet run
Access the app at: http://localhost:5000 (or configured port).

🗂 Project Structure
bash
Copy
Edit
RouteBuddy/
├── Controllers/         # MVC Controllers (Bus, User, Booking, etc.)
├── Models/              # Entity Framework Core Models
├── Views/               # Razor Views (UI Pages)
├── Migrations/          # EF Core Database Migrations
├── wwwroot/             # Static files (CSS, JS, Images)
├── layout.html          # Common layout (Header/Footer)
└── README.md            # Project Documentation
🔮 Future Roadmap
 Real-time seat availability sync.

 Bus operator portal for route & schedule management.

 AI-based fare prediction (dynamic pricing engine).

 Deployment on Azure/AWS for scalability.

 Unit Testing with xUnit & NUnit for reliability.

👨‍💻 Contributing
Contributions are welcome! Fork the repo, make changes in a new branch, and submit a PR.

📜 License
This project is licensed under the MIT License.

✨ Author
Developed by Team RouteBuddy
✨ Author
Developed by Sujith Kumar S
Aspiring Backend Developer | Python, C#, .NET Core, SQL Server
