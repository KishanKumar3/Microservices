# Microservices-based E-commerce Application

## Overview

Welcome to the GitHub repository for the microservices-based e-commerce application developed by Kishan Kumar. This application is designed to handle various aspects of e-commerce, including product management, inventory management, shopping cart functionality, order processing, and user authentication. The microservices architecture ensures modularity, scalability, and ease of maintenance.

## Microservices

### 1. Product Service

**Description:** Manages product-related operations.

**Functionality:**
- Retrieve details of all products.
- Admin-only operations: add, remove, and update product details.
- Display all products to users.

### 2. Inventory Service

**Description:** Responsible for inventory management.

**Functionality:**
- Admin can handle product quantity using the product ID.

### 3. Cart Service

**Description:** Manages user shopping carts.

**Functionality:**
- Add, view, and delete items in the cart.
- Checkout to place an order.
- Fetch product details from the Product Service using RabbitMQ.
- Verify item availability with the Inventory Service.
- Update inventory quantity after checkout.

### 4. Order Service

**Description:** Handles user orders.

**Functionality:**
- Fetch order details after checkout.

### 5. Identity Server

**Description:** Utilizes IdentityServer4 for authentication.

**Functionality:**
- Authentication and authorization.
- Adds claims to access tokens for user role and ID.

## Infrastructure Components

### 1. API Gateway

**Description:** Uses Ocelot for creating an API Gateway.

**Security:** Secured with IdentityServer.

**Load Balancing:** Configured for microservices.

### 2. Service Discovery

**Description:** Uses Eureka Server for service discovery.

**Integration:** Integrated with API Gateway.

### 3. Database

**Microservices:** MongoDB for data storage.

**Identity Server:** SQL Server for authentication data.

### 4. Message Broker

**Type:** RabbitMQ for asynchronous communication.

### 5. Common Project

**Description:** A shared project containing reusable code for all microservices.

**Principle:** Follows the DRY (Don't Repeat Yourself) principle.

### 6. Packages

**Type:** NuGet packages.

**Repository:** Published to my private GitHub repository for project usage.

### 7. Infrastructure as Code

**Docker Compose File:** Included for easy deployment of all required services. It can be found inside the “Ecom.Infra” folder.
