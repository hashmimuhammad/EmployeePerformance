const apiBaseUrl = "http://localhost:5095/api"; // Change this if your API is hosted elsewhere

// Load Employees
//async function loadEmployees() {
//    const response = await fetch(`${apiBaseUrl}/employees`);
//    const employees = await response.json();

//    const tableBody = document.getElementById("employeeTable");
//    tableBody.innerHTML = "";

//    employees.forEach(emp => {
//        const row = `
//            <tr>
//                <td>${emp.employeeId}</td>
//                <td>${emp.fullName}</td>
//                <td>${emp.email}</td>
//                <td>${emp.department}</td>
//                <td>${emp.currentSalary} ${emp.incrementApplied ? "💰" : ""}</td>
//                <td>${emp.isActive ? "Active" : "Inactive"}</td>
//                <td>
//                    <button onclick="applyIncrement(${emp.employeeId})">Apply Increment</button>
//                    <button onclick="softDeleteEmployee(${emp.employeeId})">Delete</button>
//                </td>
//            </tr>
//        `;
//        tableBody.innerHTML += row;
//    });
//}


// Search Function
function filterEmployees() {
    const searchTerm = document.getElementById("searchInput").value.toLowerCase();
    const rows = document.querySelectorAll("#employeeTable tr");

    rows.forEach(row => {
        const name = row.cells[1].textContent.toLowerCase();
        const email = row.cells[2].textContent.toLowerCase();
        const department = row.cells[3].textContent.toLowerCase();

        if (name.includes(searchTerm) || email.includes(searchTerm) || department.includes(searchTerm)) {
            row.style.display = "";
        } else {
            row.style.display = "none";
        }
    });
}

async function loadEmployees() {
    const response = await fetch(`${apiBaseUrl}/employees`);
    const employees = await response.json();

    const tableBody = document.getElementById("employeeTable");
    tableBody.innerHTML = "";

    employees.forEach(emp => {
        const isActiveText = emp.isActive ? "Active" : "Inactive";
        const isActiveColor = emp.isActive ? "green" : "red";
        const actionButton = emp.isActive
            ? `<button onclick="toggleEmployeeStatus(${emp.employeeId}, false)">Deactivate</button>`
            : `<button onclick="toggleEmployeeStatus(${emp.employeeId}, true)">Reactivate</button>`;

        const row = `
            <tr id="employee-${emp.employeeId}">
                <td>${emp.employeeId}</td>
                <td>${emp.fullName}</td>
                <td>${emp.email}</td>
                <td>${emp.department}</td>
                <td>${emp.currentSalary} ${emp.incrementApplied ? "💰" : ""}</td>
                <td class="isActive" style="color: ${isActiveColor};">${isActiveText}</td>
                <td>
                    <button onclick="applyIncrement(${emp.employeeId})" ${!emp.isActive ? "disabled" : ""}>Apply Increment</button>
                    ${actionButton}
                </td>
            </tr>
        `;
        tableBody.innerHTML += row;
    });
}
// Add Employee
//document.getElementById("employeeForm").addEventListener("submit", async (e) => {
//    e.preventDefault();

//    const employee = {
//        fullName: document.getElementById("fullName").value,
//        email: document.getElementById("email").value,
//        dateOfBirth: document.getElementById("dateOfBirth").value,
//        department: document.getElementById("department").value,
//        currentSalary: parseFloat(document.getElementById("currentSalary").value)
//    };

//    await fetch(`${apiBaseUrl}/employees`, {
//        method: "POST",
//        headers: { "Content-Type": "application/json" },
//        body: JSON.stringify(employee)
//    });

//    loadEmployees();
//});


document.getElementById("employeeForm").addEventListener("submit", async (e) => {
    e.preventDefault();

    const employee = {
        fullName: document.getElementById("fullName").value,
        email: document.getElementById("email").value,
        dateOfBirth: document.getElementById("dateOfBirth").value,
        department: document.getElementById("department").value,
        currentSalary: parseFloat(document.getElementById("currentSalary").value)
    };

    const response = await fetch(`${apiBaseUrl}/employees`, {
        method: "POST",
        headers: { "Content-Type": "application/json" },
        body: JSON.stringify(employee)
    });

    if (response.ok) {
        alert("Employee added successfully!");
        loadEmployees(); // Reload all employees (Active & Inactive)
        document.getElementById("employeeForm").reset(); // Clear the form after adding
    } else {
        const errorMessage = await response.text();
        alert("Error: " + errorMessage);
    }
});





// Soft Delete Employee
//async function softDeleteEmployee(id) {
//    await fetch(`${apiBaseUrl}/employees/${id}`, { method: "DELETE" });
//    loadEmployees();
//}

//async function softDeleteEmployee(id) {
//    const response = await fetch(`${apiBaseUrl}/employees/${id}`, {
//        method: "DELETE"
//    });

//    if (response.ok) {
//        alert("Employee deactivated successfully!");

//        // Update the row's IsActive column in the UI
//        const row = document.querySelector(`#employee-${id}`);
//        if (row) {
//            const isActiveCell = row.querySelector(".isActive");
//            isActiveCell.textContent = "Inactive"; // Change status in the table
//            isActiveCell.style.color = "red"; // Optional: Change color for visibility
//        }
//    } else {
//        const errorMessage = await response.text();
//        alert("Error: " + errorMessage);
//    }
//}

//async function softDeleteEmployee(id) {
//    const response = await fetch(`${apiBaseUrl}/employees/${id}`, {
//        method: "DELETE"
//    });

//    if (response.ok) {
//        alert("Employee deactivated successfully!");

//        // Update the row's IsActive column in the UI
//        const row = document.querySelector(`#employee-${id}`);
//        if (row) {
//            const isActiveCell = row.querySelector(".isActive");
//            isActiveCell.textContent = "Deactivated"; // Change status in the table
//            isActiveCell.style.color = "red"; // Optional: Change color for visibility

//            // Update the button to "Reactivate"
//            const actionButton = row.querySelector(".toggleStatusButton");
//            actionButton.textContent = "Reactivate";
//            actionButton.setAttribute("onclick", `reactivateEmployee(${id})`);
//        }
//    } else {
//        const errorMessage = await response.text();
//        alert("Error: " + errorMessage);
//    }
//}

async function softDeleteEmployee(id) {
    const response = await fetch(`${apiBaseUrl}/employees/${id}`, {
        method: "DELETE"
    });

    if (response.ok) {
        alert("Employee deactivated successfully!");

        // Find the row using the employee ID
        const rows = document.querySelectorAll("#employeeTable tr");
        rows.forEach(row => {
            const employeeIdCell = row.cells[0]; // First cell contains employee ID
            if (employeeIdCell && employeeIdCell.textContent == id) {
                const isActiveCell = row.cells[5]; // 6th column (zero-based index 5)
                isActiveCell.textContent = "Inactive"; // Update status
                isActiveCell.style.color = "red"; // Change text color

                // Disable the delete button
                const deleteButton = row.cells[6].querySelector("button:last-child");
                deleteButton.disabled = true;
                deleteButton.textContent = "Deactivated";
            }
        });
    } else {
        const errorMessage = await response.text();
        alert("Error: " + errorMessage);
    }
}





// Apply Salary Increment
async function applyIncrement(id) {
    const response = await fetch(`${apiBaseUrl}/employees/${id}/increment`, { method: "PUT" });
    const data = await response.json();
    alert(data.message);
    loadEmployees();
}

// Load Reviews
// Load All Reviews
async function loadReviews() {
    try {
        const response = await fetch(`${apiBaseUrl}/performanceReviews`);
        if (!response.ok) throw new Error("Failed to load reviews");

        const reviews = await response.json();
        console.log("Reviews:", reviews); // Debugging

        const tableBody = document.getElementById("reviewTable");
        tableBody.innerHTML = "";

        reviews.forEach(review => {
            const row = `
                <tr>
                    <td>${review.employeeId}</td>
                    <td>${review.perfomanceScore}</td>
                    <td>${review.comments}</td>
                    <td><button onclick="deleteReview(${review.reviewId})">Delete</button></td>
                </tr>
            `;
            tableBody.innerHTML += row;
        });
    } catch (error) {
        console.error("Error loading reviews:", error);
        alert("Error loading reviews.");
    }
}


// Add Review
//document.getElementById("reviewForm").addEventListener("submit", async (e) => {
//    e.preventDefault();

//    const employeeId = document.getElementById("employeeId").value.trim();
//    const perfomanceScore = document.getElementById("perfomanceScore").value.trim(); // Kept typo as in model
//    const comments = document.getElementById("comments").value.trim();

//    // Validate Employee ID (Must be a valid number)
//    if (!employeeId || isNaN(employeeId)) {
//        alert("Invalid Employee ID. Please enter a valid number.");
//        return;
//    }

//    // Validate Performance Score (Must be between 1 and 10)
//    if (!perfomanceScore || isNaN(perfomanceScore) || perfomanceScore < 1 || perfomanceScore > 10) {
//        alert("Performance Score must be between 1 and 10.");
//        return;
//    }

//    // Create Review Object (Kept property names exactly as in model)
//    const review = {
//        employeeId: parseInt(employeeId, 10),
//        perfomanceScore: parseInt(perfomanceScore, 10), // Kept typo as in your model
//        comments: comments
//    };

//    try {
//        console.log("Sending review:", JSON.stringify(review));

//        const response = await fetch("http://localhost:5095/api/PerformanceReviews", {
//            method: "POST",
//            headers: { "Content-Type": "application/json" },
//            body: JSON.stringify(review)
//        });

//        console.log("Response status:", response.status);
//        const responseText = await response.text();
//        console.log("Response body:", responseText);

//        if (!response.ok) {
//            throw new Error(responseText || "Failed to add review");
//        }

//        alert("Review added successfully!");
//        loadReviews(); // Refresh the reviews list
//    } catch (error) {
//        console.error("Error:", error);
//        alert("Error adding review: " + error.message);
//    }
//});



document.getElementById("reviewForm").addEventListener("submit", async (e) => {
    e.preventDefault();

    const employeeId = document.getElementById("employeeId").value.trim();
    const perfomanceScore = document.getElementById("perfomanceScore").value.trim(); // Kept typo as in model
    const comments = document.getElementById("comments").value.trim();

    // Validate Employee ID (Must be a valid number)
    if (!employeeId || isNaN(employeeId)) {
        alert("Invalid Employee ID. Please enter a valid number.");
        return;
    }

    // Fetch employee data to check if they are active
    try {
        const empResponse = await fetch(`http://localhost:5095/api/employees/${employeeId}`);
        if (!empResponse.ok) {
            alert("Employee not found.");
            return;
        }

        const employee = await empResponse.json();
        if (!employee.isActive) {
            alert("Cannot add review. Employee is inactive.");
            return;
        }
    } catch (error) {
        console.error("Error fetching employee:", error);
        alert("Error checking employee status.");
        return;
    }

    // Validate Performance Score (Must be between 1 and 10)
    if (!perfomanceScore || isNaN(perfomanceScore) || perfomanceScore < 1 || perfomanceScore > 10) {
        alert("Performance Score must be between 1 and 10.");
        return;
    }

    // Create Review Object (Kept property names exactly as in model)
    const review = {
        employeeId: parseInt(employeeId, 10),
        perfomanceScore: parseInt(perfomanceScore, 10), // Kept typo as in your model
        comments: comments
    };

    try {
        console.log("Sending review:", JSON.stringify(review));

        const response = await fetch("http://localhost:5095/api/PerformanceReviews", {
            method: "POST",
            headers: { "Content-Type": "application/json" },
            body: JSON.stringify(review)
        });

        console.log("Response status:", response.status);
        const responseText = await response.text();
        console.log("Response body:", responseText);

        if (!response.ok) {
            throw new Error(responseText || "Failed to add review");
        }

        alert("Review added successfully!");
        loadReviews(); // Refresh the reviews list
    } catch (error) {
        console.error("Error:", error);
        alert("Error adding review: " + error.message);
    }
});



// Delete Review
async function deleteReview(id) {
    await fetch(`${apiBaseUrl}/performanceReviews/${id}`, { method: "DELETE" });
    loadReviews();
}

// Load data on page load
document.addEventListener("DOMContentLoaded", () => {
    loadEmployees();
    loadReviews();
});
