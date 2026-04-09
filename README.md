# Unity-Template-Project
Strucktur Project
- Startup (App Bootstrap, Auto SignIn)
- Auth Menu (Login, Register, Token)
- Lobby Menu (User Profile)

Feature
- MVP (Model View Presenter)
- UniTask
- DI (VContainer)
- Assembly Definitions
- Unit Testing

Service
- Navigation Service (Route panel and scene)
- API Service
- Addressable Asset
- Global UI
- Audio Service
- Config (.env.dev & .env.prod)
  
![Architecture Diagram](./Template-Base-Flow.drawio.svg)

Create an environment (.env) file to connect the game with your game API

Example : (.env.dev)
  - API_BASE_URL=https://link/game
  - API_BEARER=token
  - API_SALT_PASSWORD=salt
  - WEBSOCKET_URL=wss://link/game


Additional Service :
- Firebase (authentication & Messaging)
- Google SignIn
  
note* add the SDKs (firebase & google signin) if you want those additional service to work
