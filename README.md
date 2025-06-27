# Auto Moderated Forum

## Description
**Auto Moderated Forum** is a web application for sharing comments that automatically detects and filters inappropriate content using a machine learning model trained with ML.NET.

---

## Contents
- [Technologies and Libraries Used](#technologies-and-libraries-used)  
- [Roles and Access Rights](#roles-and-access-rights)  
- [User Management and Authentication](#user-management-and-authentication)  
- [Database Models](#database-models)  
- [Database Context](#database-context)  
- [Enumerations](#enumerations)  
- [Role and Admin Initialization](#role-and-admin-initialization)  
- [Machine Learning Model](#machine-learning-model)  
- [Controllers](#controllers)  
- [Future Improvements](#future-improvements)  

---

## Technologies and Libraries Used
- **ASP.NET Core Identity** – User authentication and authorization  
- **Entity Framework Core** – Database interaction  
- **SQL Server** – Stores users, moderation requests, comments, etc.  
- **ML.NET** – Machine learning library for training and applying the moderation model  
- **ASP.NET Core MVC / Razor Pages** – UI and HTTP request handling  

---

## Roles and Access Rights
- `Admin`  
- `Moderator`  
- `User`  

---

## User Management and Authentication
ASP.NET Core Identity is used for managing user authentication and authorization. The project includes registration, login, and logout functionality using Identity scaffolding.

---

## Database Models

### `Comment`
- Text  
- Creation date  
- Author (user)  
- IsApproved  
- IsFlagged  

### `ModerationRequest`
- Associated comment  
- Moderator  
- Review date  
- Status: `Pending`, `Approved`, or `Rejected`  
- Optional notes  

---

## Database Context

### `ApplicationDbContext`
- Inherits from `IdentityDbContext`  
- Contains `DbSet` properties for all application models  
- Relationships configured in `OnModelCreating()`  

---

## Enumerations

### `ModelDecision`
- `Pending` – flagged by ML model, awaits moderator review  
- `Approved` – approved by moderator, publicly visible  
- `Rejected` – rejected as inappropriate, hidden/deleted  

---

## Role and Admin Initialization

### `SeedData.InitializeAsync(IServiceProvider serviceProvider)`
- Checks and creates the roles: `User`, `Admin`, `Moderator`  
- Ensures a default admin exists (`admin@forum.com`)  
- Called in `Program.cs` on app startup  

---

## Machine Learning Model

### `TrainModelApp` (separate training project)
- Uses `comments.csv` (comment, isRude) as training data  
- Text is transformed in a pipeline  
- Algorithm: `SdcaLogisticRegression` for binary classification  
- Model saved as `nas-bert-model.zip`

### `SentimentAnalysisService`
- Loads the saved model  
- Evaluates new comments  
- Flags inappropriate comments automatically  
- Hidden comments await manual moderator review  

---

## Controllers

### `AdminController`
- Restricted to Admins  
- Manage users and their roles  
  - `Index()` – List users  
  - `AddRole(userId, role)`  
  - `RemoveRole(userId, role)`  

### `HomeController`
- Handles Index and Privacy pages  

### `CommentsController`
- Add new comments  
- Automatic evaluation using ML model  
- Display public and personal comments  
- Moderation by moderators  

#### Key Methods:
- `AddComment(string content)`  
- `UserComments()`  
- `Index()`  
- `Flagged()`  
- `Reviewed()`  
- `ApproveComment(id)`  
- `DeleteComment(id)`  
- `ModerateComment(id, decision, notes)`  

---

## Future Improvements
- User notifications for comment status  
- Support for posts with nested comments  
- Replies and threaded discussions  
- Likes and comment reporting  
- Categorized moderation (e.g., offensive, threatening)  
- Two-factor authentication  
- Mobile-responsive design  

---

## License
This project is for educational purposes.  
