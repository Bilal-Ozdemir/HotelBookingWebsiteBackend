// js/api/api.js
const apiBaseURL = "http://your-backend-url/api"; 
// Function to get available rooms
export async function getAvailableRooms() {
    try {
        const response = await fetch(`${apiBaseURL}/booking`);
        if (!response.ok) {
            throw new Error("Failed to fetch available rooms");
        }
        return await response.json();
    } catch (error) {
        console.error("Error in fetching rooms:", error);
        throw error;
    }
}

// Function to create a booking
export async function createBooking(bookingDetails) {
    try {
        const response = await fetch(`${apiBaseURL}/booking`, {
            method: 'POST',
            headers: {
                'Content-Type': 'application/json'
            },
            body: JSON.stringify(bookingDetails)
        });

        if (!response.ok) {
            throw new Error("Failed to create booking");
        }
        return await response.json();
    } catch (error) {
        console.error("Error in creating booking:", error);
        throw error;
    }
}
