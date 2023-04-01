# MinimalRPC
An opinionated starter project using Minimal APIs with RPC style endpoints following the Request-Endpoint-Response (REPR) pattern.

## Opinionated Design Decisions
1. All endpoints are a `HTTP POST` with a `JSON Request Body`
2. Follows the [REPR Pattern](https://ardalis.com/mvc-controllers-are-dinosaurs-embrace-api-endpoints/) where each endpoint is its own class with its own request/response.
3. Only the following Status Codes are allowed: `200 OK`, `400 Bad Request`, '401 Unathorized', `500 Server Error`

## Why RPC over REST?
In my opinion and personal experience, RPC api's are:
- Easier to develop since everything is a `HTTP POST`, you only need to use work with a `JSON Request Body` and no longer need route params, query params etc.
- Easier to consume since routes are more descriptive like method names and everything required is in the body.
  - RPC: `HTTP POST /GetTodoItemComments`
  - REST: `HTTP GET /todo-items/{id}/comments`
