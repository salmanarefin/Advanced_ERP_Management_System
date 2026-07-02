# ERP HRMS Management System

A role-based Enterprise Resource Planning and Human Resource Management System built with **ASP.NET Core MVC**, **Entity Framework Core**, **SQL Server**, and **ASP.NET Core Identity**.

This system helps manage employees, departments, designations, attendance, leave applications, payroll, reports, audit logs, and user roles from one secure dashboard.

---

## Project Overview

ERP HRMS Management System is a web-based application designed for managing core HR operations in an organization. The system includes role-based access control, professional dashboard charts, employee management, attendance tracking, leave management, payroll processing, reporting, and audit logging.

The project follows a **folder-based 3-tier architecture** where controllers and views handle the presentation layer, services handle business logic, and Entity Framework Core handles database operations.

---

## Key Features

### Authentication and Authorization

- ASP.NET Core Identity authentication
- Role-based access control
- Custom login page
- Custom register page
- Custom access denied page
- Default role assignment for newly registered users
- Admin-controlled user role management

### Dashboard

- Summary cards for employees, departments, designations, attendance, leave, and payroll
- Employees by department pie chart
- Attendance trend line chart
- Quick HRMS summary section

### HR Setup

- Department management
- Designation management
- Leave type management
- Active/inactive status support
- Professional create, edit, details, and delete pages

### Employee Management

- Employee create, edit, details, and delete/deactivate
- Search and pagination
- Department and designation assignment
- Employee status management
- Duplicate employee code/email validation
- Salary and joining date validation

### Attendance Management

- Employee attendance tracking
- Check-in and check-out time
- Working hour calculation support
- Attendance status: Present, Absent, Late, Half Day
- Validation for future date and invalid time range

### Leave Management

- Leave application system
- Leave type selection
- Start date and end date validation
- Leave approval/rejection status
- Rejection reason support
- Leave reports with filters

### Payroll Management

- Monthly payroll generation
- Basic salary, bonus, deduction, tax, and net salary
- Payroll report with summary cards
- Grand total row
- Month/year filter
- Employee filter
- Search support

### Reports

- Employee report
- Attendance report
- Leave report
- Payroll report
- Date filters
- Status filters
- Employee filters
- Search filters
- Print-friendly report layout

### Audit Logs

- System activity tracking
- User email tracking
- Action tracking
- Module and table tracking
- IP address tracking
- Search and pagination
- Admin-only access

### Professional UI

- Custom dashboard layout
- Role-based sidebar visibility
- Active sidebar menu highlight
- Professional login/register UI
- Consistent card-based pages
- Bootstrap-based responsive design

---

## Role-Based Access Control

| Feature / Module | Admin | HR | Manager | Employee | PayrollOfficer |
|---|---:|---:|---:|---:|---:|
| Dashboard | Yes | Yes | Yes | Yes | Yes |
| Departments | Yes | Yes | No | No | No |
| Designations | Yes | Yes | No | No | No |
| Leave Types | Yes | Yes | No | No | No |
| Employees | Yes | Yes | No | No | No |
| Attendance | Yes | Yes | Yes | No | No |
| Leave Applications | Yes | Yes | Yes | Yes | No |
| Payroll | Yes | No | No | No | Yes |
| Reports | Yes | Yes | Yes | No | Yes |
| User Roles | Yes | No | No | No | No |
| Audit Logs | Yes | No | No | No | No |

---

## Technology Stack

### Backend

- ASP.NET Core MVC
- C#
- Entity Framework Core
- ASP.NET Core Identity
- LINQ
- Dependency Injection

### Frontend

- Razor Views
- HTML5
- CSS3
- Bootstrap
- JavaScript
- Chart.js

### Database

- SQL Server / SQL Server LocalDB
- Entity Framework Core Migrations

### Tools

- Visual Studio 2022
- Git
- GitHub

---

## Architecture

This project follows a **folder-based 3-tier architecture**.

```text
ERP_version1
├── Controllers          # Presentation flow
├── Views                # UI pages
├── ViewModels           # Data transfer for views
├── Services             # Business logic layer
├── Models               # Entity models
├── Data                 # ApplicationDbContext
├── Helpers              # Seeder/helper classes
├── Migrations           # EF Core migrations
└── wwwroot              # CSS, JS, static files
