/**
 * Cogtive Industrial IoT Simulator
 * 
 * This simulator generates random production data for industrial machines and sends it to the API.
 * It simulates multiple IoT devices on the factory floor reporting metrics.
 * 
 * Senior-level candidates can extend this simulator as part of the challenge.
 */

const axios = require('axios');

// Configuration
const API_ENDPOINT = process.env.API_ENDPOINT || 'http://localhost:5211/api/production-data';
const SEND_INTERVAL_MS = 10000; // Send data every 10 seconds
const MACHINES = [
    { id: 1, name: 'CNC Machine Alpha' },
    { id: 2, name: 'Injection Molder Beta' },
    { id: 3, name: 'Assembly Line Gamma' }
];

// Helper function to generate random production data
function generateProductionData(machineId) {
    // Generate random efficiency between 70-100%
    const efficiency = parseFloat((70 + Math.random() * 30).toFixed(1));
    
    // Generate random units produced
    const unitsProduced = Math.floor(100 + Math.random() * 500);
    
    // Generate random downtime (0-60 minutes)
    const downtime = Math.floor(Math.random() * 60);
    
    return {
        machineId,
        timestamp: new Date().toISOString(),
        // Fixed: Efficiency is now a number, not a string
        efficiency: efficiency,
        unitsProduced,
        downtime
    };
}

// Function to send data to API
async function sendData(data) {
    try {
        const response = await axios.post(API_ENDPOINT, data);
        console.log(`✅ Data sent successfully for machine ${data.machineId}: Efficiency: ${data.efficiency}%, Units: ${data.unitsProduced}`);
        return response.data;
    } catch (error) {
        console.error(`❌ Error sending data for machine ${data.machineId}:`, error.message);
        return null;
    }
}

// Main function that runs the simulation
function runSimulation() {
    console.log('🏭 Starting Cogtive Industrial IoT Simulator');
    console.log(`📡 Sending data to: ${API_ENDPOINT}`);
    console.log(`⏱️ Interval: ${SEND_INTERVAL_MS / 1000} seconds\n`);
    
    // Send data for each machine at regular intervals
    setInterval(() => {
        MACHINES.forEach(machine => {
            // Only send data 70% of the time to simulate real-world conditions
            if (Math.random() > 0.3) {
                const data = generateProductionData(machine.id);
                sendData(data);
            }
        });
    }, SEND_INTERVAL_MS);
}

// Run the simulation
runSimulation();
