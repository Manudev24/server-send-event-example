# server-send-event-example

This project is a web application in C# using ASP.NET Core to provide an API with two main endpoints. The application allows clients to connect via Server-Sent Events (SSE) and send messages to these clients.

## Endpoints

### 1. SSE Connection

**URL:** `/sse/{id}`  
**Method:** `GET`  
**Description:** This endpoint allows clients to connect to the server using Server-Sent Events (SSE). Each client is identified by a unique `id`.

**Parameters:**
- `id` (string): Unique identifier of the client.

**Request Example:**

```
POST /send Content-Type: application/json

{ "id": "client1", "message": "Hello, client1!" }
```

**GET:** `/sse/{id}`  

**Response:**
- `200 OK`: The connection is successfully established, and the client starts receiving real-time messages.
- `Content-Type: text/event-stream`: The response is sent in SSE event format.

### 2. Send Messages

**URL:** `/send`  
**Method:** `POST`  
**Description:** This endpoint allows sending messages to a specific client identified by their `id`.

**Parameters:**
- `id` (string): Unique identifier of the client.
- `message` (string): Message to be sent to the client.

**Request Example:**


**Response:**
- `200 OK`: The message was successfully sent to the client.
- `404 Not Found`: No client found with the specified `id`.


