# Indentity Provider  Project

## Overview

This project is an implementation of IdentityServer, an open-source framework for managing authentication and authorization using OAuth 2.0 and OpenID Connect standards. It provides secure token issuance and user authentication services for protecting APIs and managing access to resources.

## Features

- **User Authentication**: Supports user authentication using ASP.NET Core Identity.
- **Token Issuance**: Issues JWT access tokens, ID tokens, and refresh tokens.
- **OAuth 2.0 and OpenID Connect Support**: Implements industry-standard protocols for secure authentication and authorization.
- **API Resource Protection**: Manages API access and protects resources with fine-grained access control.
- **Customizable**: Easily extendable to add custom claims, user profiles, and other identity-related features.

## Requirements

- **.NET Core SDK 6.0 or higher**: The project is built using .NET Core 6.0.
- **Visual Studio 2022 or Visual Studio Code**: For development and running the project.
- **SQL Server or SQLite**: For storing user data if using ASP.NET Core Identity.

## Getting Started

### 1. Clone the Repository

Clone the repository to your local machine using Git:

```bash
git clone https://github.com/FilipTrifunovic/indentity-provider.git
cd indentity-provider 
