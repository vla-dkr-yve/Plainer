const showPopup = msg => {
  const p = document.getElementById("popup");
  p.innerText = msg;
  p.style.display = "block";
  setTimeout(() => (p.style.display = "none"), 3000);
};

const toggleDropdown = () => {
  const d = document.getElementById("userDropdown");
  d.style.display = d.style.display === "block" ? "none" : "block";
};

document.getElementById("userMenu").addEventListener("click", toggleDropdown);

const showSection = id => {
  ["registerForm", "loginForm", "eventList", "addEventForm", "eventDetails"].forEach(s =>
    document.getElementById(s).style.display = "none"
  );
  document.getElementById(id).style.display = "block";
};

const showLogin = () => showSection("loginForm");
const showRegister = () => showSection("registerForm");

const showEventList = () => {
  showSection("eventList");
  fetchEvents();
};

const showAddEvent = () => showSection("addEventForm");

const register = async () => {
  const res = await fetch("http://localhost:5238/users/register", {
    method: "POST",
    headers: { "Content-Type": "application/json" },
    body: JSON.stringify({
      username: document.getElementById("regUsername").value,
      password: document.getElementById("regPassword").value
    })
  });
  if (res.ok) {
    showPopup("Registered! Please login.");
    showLogin();
  } else {
    showPopup("Registration failed");
  }
};

const login = async () => {
  const res = await fetch("http://localhost:5238/users/login", {
    method: "POST",
    headers: { "Content-Type": "application/json" },
    body: JSON.stringify({
      username: document.getElementById("loginUsername").value,
      password: document.getElementById("loginPassword").value
    })
  });
  const data = await res.json();
  if (res.ok) {
    token = data.token;
    username = document.getElementById("loginUsername").value;
    localStorage.setItem("token", token);
    localStorage.setItem("username", username);
    document.getElementById("userName").innerText = username;
    document.getElementById("userMenu").style.display = "block";
    showEventList();
  } else {
    showPopup("Login failed");
  }
};

const logout = () => {
  localStorage.clear();
  token = null;
  username = null;
  document.getElementById("userMenu").style.display = "none";
  showRegister();
};

const fetchEvents = async () => {
  const res = await fetch("http://localhost:5238/events", {
    headers: { Authorization: `Bearer ${token}` }
  });
  const data = await res.json();
  const container = document.getElementById("eventsContainer");
  container.innerHTML = "";
  data.forEach(ev => {
    const div = document.createElement("div");
    div.className = "event-card";
    div.innerHTML = `
      <h3>${ev.title}</h3>
      <p>Description: ${ev.description}<p>
      <p>Start time: ${new Date(ev.startTime).toLocaleString()}</p>
      <p>End time: ${new Date(ev.endTime).toLocaleString()}</p>
      <p>Number of participants: ${ev.numberOfParticipants}</p>
      <button onclick="openEvent(${ev.id})">Open</button>
    `;
    container.appendChild(div);
  });
};

const addParticipantToEvent = async (eventId, event) => {
  if (event) event.preventDefault();
  const usernameToAdd = prompt("Enter username to add:");
  if (!usernameToAdd) return;
  try {
    const res = await fetch(`http://localhost:5238/events/${eventId}/participants/${usernameToAdd}/`, {
  method: "POST",
  headers: { Authorization: `Bearer ${token}` }
})
    if (res.ok) {
      showPopup("User added successfully!");
      openEvent(eventId);
    } else {
      const msg = await res.text();
      showPopup("Failed to add user: " + msg);
    }
  } catch (err) {
    console.error("Add error:", err);
    showPopup("Server error while adding user");
  }
};

const removeParticipantFromEvent = async (eventId, userIdToRemove, event) => {
  if (event) event.preventDefault();
  if (!confirm("Are you sure you want to remove this participant?")) return;
  try {
    const res = await fetch(`http://localhost:5238/events/${eventId}/participants/${userIdToRemove}/`, {
      method: "DELETE",
      headers: {
        "Content-Type": "application/json",
        Authorization: `Bearer ${token}`
      },
    });
    if (res.ok) {
      showPopup("User removed successfully!");
      openEvent(eventId);
    } else {
      const msg = await res.text();
      showPopup("Failed to remove user: " + msg);
    }
  } catch (err) {
    console.error("Remove error:", err);
    showPopup("Server error while removing user");
  }
};

    const fetchCategories = async () => {
        try {
        const res = await fetch(`http://localhost:5238/categories`,{
        headers: { Authorization: `Bearer ${token}` }
            }
        );
    const categories = await res.json();
    const select = document.getElementById("categorySelect");

    select.innerHTML = `<option value="">-</option>`;
    categories.forEach(cat => {
        const option = document.createElement("option");
        option.value = cat;
        option.textContent = cat;
        select.appendChild(option);
    });
    } 
    catch (err) {
        console.error("Failed to load categories:", err);
        showPopup("Could not load categories.");
    }
}
const openEvent = async id => {
    const res = await fetch(`http://localhost:5238/events/${id}`, {
        headers: { Authorization: `Bearer ${token}` }
    });
    const ev = await res.json();
    const detail = document.getElementById("eventDetails");
    const currentUserId = parseInt(parseJwt(token).nameid);
    detail.innerHTML = `
        <h2>${ev.title}</h2>
        <p>${ev.description}</p>
        <p><strong>Start:</strong> ${new Date(ev.startTime).toLocaleString()}</p>
        <p><strong>End:</strong> ${new Date(ev.endTime).toLocaleString()}</p>
        <p><strong>Category:</strong> ${ev.categoryName}</p>
        <p><strong>Created By:</strong> ${ev.createdBy}</p>
        <h4>Participants:</h4>
        <ul>
            ${ev.eventParticipants.map(p => {
              const isOtherUser = p.userId !== currentUserId;
              const roleOptions = [
                { id: 4, name: "-" },
                { id: 3, name: "Participant" },
                { id: 2, name: "Manager" }
              ];
              return `
                <li>
                  User: ${p.userName} | Current Role: ${p.roleName}
                  ${isOtherUser ? `
                    <select onchange="updateSelectedRole(${p.userId}, this.value)">
                      ${roleOptions.map(r => `
                        <option value="${r.id}" ${r.name === p.roleName ? "selected" : ""}>${r.name}</option>
                      `).join("")}
                    </select>
                    <button onclick="changeRole(${ev.id}, ${p.userId})">Change Role</button>
                    <button onclick="removeParticipantFromEvent(${ev.id}, ${p.userId}, event)">Remove</button>
                  ` : ""}
                </li>`;
        }).join("")}
    </ul>
    <button onclick="addParticipantToEvent(${ev.id}, event)">Add Participant</button>
    <br><br>
    <button onclick="showEventList()">Back</button>`;

    showSection("eventDetails");
};

const createNewEvent = async () => {
    let selectedCategoryValue = document.getElementById("categorySelect").value || null;
    console.log(selectedCategoryValue);
    const categoryOptions = [
      { name: null, id: 0},
      { name: "Work", id: 1 },
      { name: "Personal", id: 2 },
      { name: "Holiday", id: 3 },
      { name: "Sport", id: 4 }
    ];
    let selectedCategory = categoryOptions.find(cat => cat.name === selectedCategoryValue).id;
    console.log(selectedCategory);
    const res = await fetch("http://localhost:5238/events", {
        method: "POST",
        headers: {
            "Content-Type": "application/json",
            Authorization: `Bearer ${token}`
        },
        body: JSON.stringify({
            Title: document.getElementById("title").value,
            Description: document.getElementById("description").value,
            StartTime: document.getElementById("startTime").value,
            EndTime: document.getElementById("endTime").value,
            CategoryId: selectedCategory
            })
        });
    if (res.ok) {
        showPopup("Event Created!");
        showEventList();
    } else {
        showPopup("Failed to create event");
    }
}

let selectedRoles = {};
function updateSelectedRole(userId, roleId) {
    selectedRoles[userId] = parseInt(roleId);
}

async function changeRole(eventId, targetUserId) {
const newRoleId = selectedRoles[targetUserId];
console.log(newRoleId);
    if (!newRoleId || newRoleId == 4) {
        showPopup("Please select a role before submitting.");
        return;
    }
    try {
        const res = await fetch(`http://localhost:5238/events/${eventId}/participants/${targetUserId}/role`, {
            method: "POST",
            headers: {
                "Content-Type": "application/json",
                Authorization: `Bearer ${token}`
            },
        body: JSON.stringify({ newRoleId: parseInt(newRoleId) })
    });
    if (res.ok) {
        showPopup("Role updated successfully!");
        openEvent(eventId);
    } else if (res.status === 403) {
        showPopup("You are not authorized to change this role.");
    } else {
        const text = await res.text();
        showPopup("Failed to change role. Server says: " + text);
    }
    } 
    catch (err) {
        showPopup("Error connecting to server.");
        console.error(err);
    }
}
    function parseJwt(token) {
        const base64Url = token.split('.')[1];
        const base64 = base64Url.replace(/-/g, '+').replace(/_/g, '/');
        const jsonPayload = decodeURIComponent(atob(base64).split('').map(c =>
        '%' + ('00' + c.charCodeAt(0).toString(16)).slice(-2)
        ).join(''));
        return JSON.parse(jsonPayload);
    }

    function goHome() {
        if (token) {
            showEventList();
        }
        else {
            showRegister();
        }
    }

if (token) {
    document.getElementById("userMenu").style.display = "block";
    document.getElementById("userName").innerText = username;
    showEventList();
    fetchCategories();
} 
else {
    showRegister();
}
