# RooPOS

RooPOS is a lightweight **ERP & POS system** designed for small local businesses such as supermarkets, pharmacies, and retail shops.

The system focuses on **simplicity, offline-first usage, and affordability**, providing essential inventory and sales management without the complexity of traditional ERP systems.

---

## Problem Statement

Small businesses often rely on:
- Manual tracking
- Excel sheets
- Basic POS tools with no reporting

This leads to:
- Inventory errors
- No clear profit tracking
- Poor sales history
- No real insights

RooPOS solves this by offering a **simple desktop POS + ERP solution** that works offline and is easy to operate.

---

## 👥 Target Users
- Small business owners
- Local retail shops
- Cashiers and store staff

---

## System Overview

- **Frontend**: Desktop application built with **Flutter**
- **Backend**: RESTful API built with **.NET**
- **Database**: SQL Server Express (local / offline-first)
- **Architecture**: Modular, permission-based, scalable

---

## Actors & Roles

| Actor        | Description |
|-------------|-------------|
| Admin       | Full system control, configuration, reports |
| Manager     | Store & product management (per permissions) |
| StoreKeeper | Inventory handling & refill processing |
| Cashier     | Sales & order processing |

---

## 🗂 Repository Structure
```
RooPOS/
│
├── RooPOS - Backend // # .NET API
│
├── RooPOS - Desktop client // # Flutter Desktop App
│
└── README.md
```

---

## 🧠 Design Principles
- Offline-first
- Simple UX for non-technical users
- Permission-based access
- Minimal setup for local businesses
- Ready for future expansion (cloud sync, multi-store, etc.)

---

## 📌 Status
🚧 MVP – Active Development