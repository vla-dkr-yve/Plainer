# Plainer
Plainer is a full-stack web application for managing and attending events. Users can register, log in, create events, view details, and join or leave events. Events can have multiple participants, each assigned a specific role (owner, manager and participant).
# Back-end
The backend is built using ASP.NET Core Web API and Entity Framework Core, following RESTful principles. It handles all business logic, data validation, and database interaction.
### Features
1. User Authentication: Secure login with JWT tokens.
2. Event Management: Create, read, update, and delete events.
3. Event Participation: Users can be added to the event or removed from it by the higher role.
4. Role Assignment: Each participant can have a one out of three preseeded values in an event.
    Owner - full controll over the event,
    Manager - can add and remove users from the event
    Participant- can only observe information about event
5. Category Support: Events can be categorized.
6. Detailed DTO Mapping: DTOs are used to avoid serialization cycles and expose only needed data.
# Entity Realtional Diagram (ERD)
![Blank diagram](https://github.com/user-attachments/assets/8ec50810-5abc-4af8-bbb0-ec33daa0c087)
# Front-End
The frontend is built using HTML, CSS, and JavaScript, and communicates with the backend through fetch API calls. It supports:
* User registration and login
* Viewing event lists and details
* Creating and managing events
# Screen layout
Registration: ![image](https://github.com/user-attachments/assets/714483b4-69de-4a78-8102-a74d96811aad)
Login: ![image](https://github.com/user-attachments/assets/b7db6427-a42c-46cd-88c3-29af11cfb8a6)
Creating Event: ![image](https://github.com/user-attachments/assets/7e3c12b7-57f2-40ed-9543-f519d0a738f0)
Event/{id} info: ![image](https://github.com/user-attachments/assets/e321e96e-73c7-4c3b-9b94-82c1b5d5b8d3)
Events view: ![image](https://github.com/user-attachments/assets/51d5492b-3a19-4410-9bdd-4f973d8ca1d6)
Adding user to the evet: ![image](https://github.com/user-attachments/assets/7ca9cbbd-c862-4c22-8732-d4f9b11b6668)
After adding: ![image](https://github.com/user-attachments/assets/d79464a3-0a18-45d0-aa2d-798d4b29f8b5)

# Technologies used
* C#, ASP.NET Core
* Entity Framework Core
* JWT Authentication
* SQLite
* HTML, CSS, JavaScript
