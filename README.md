# SwiftMT799 API

This project is a web API designed to parse and handle MT799 messages. It allows users to upload MT799 messages either as files or direct string input and saves the parsed messages to a SQLite database. 
The API provides several endpoints to retrieve stored messages and their headers.

## Project Structure

- **Program.cs**: Entry point of the application.
- **Controllers**:
  - `MT799Controller`: Handles HTTP requests related to MT799 message processing.
- **Services**:
  - `MT799Parser`: Parses the raw MT799 message content.
  - `SQLiteHelper`: Manages database operations with SQLite.
- **Models**:
  - `MT799Message`: Represents the main structure of the MT799 message.
  - `GeneralHeaderData`: Represents block 1 header data for the MT799 message.
  - `GeneralInputHeaderData`: Represents block 2 input header data for the MT799 message.
  - `GeneralOutputHeaderData`: Represents block 2 output header data for the MT799 message.
  - Additional models for storing message header data.

## Features

- **Upload MT799 Messages**:
  - Supports file upload and direct string input.
  - Parses the uploaded content and stores it in the SQLite database.

- **Retrieve Stored Messages**:
  - Provides endpoints to fetch all stored MT799 messages and their respective headers.

## Endpoints

- **POST /MT799**: Upload an MT799 message via file or string input.
- **GET /MT799/Messages**: Retrieve all stored MT799 messages.
- **GET /MT799/HeaderMessages**: Retrieve all general header data.
- **GET /MT799/InputHeaderMessages**: Retrieve input header data.
- **GET /MT799/OutputHeaderMessages**: Retrieve output header data.

## Documentation and Testing
This API is documented using XML comments within the codebase.
The program can be testing using Swagger for a user-friendly interface for testing and exploring the API

