import axios from 'axios';
import { Machine, ProductionData } from './models';

const API_BASE = process.env.REACT_APP_API_BASE || 'http://localhost:5000';
const WS_BASE = process.env.REACT_APP_WS_BASE || 'ws://localhost:5000';

export const fetchMachines = async (): Promise<Machine[]> => {
  const response = await axios.get<Machine[]>(`${API_BASE}/api/machines`);
  return response.data;
};

export const fetchMachineById = async (id: number): Promise<Machine> => {
  const response = await axios.get<Machine>(`${API_BASE}/api/machines/${id}`);
  return response.data;
};

export const fetchProductionData = async (): Promise<ProductionData[]> => {
  const response = await axios.get<ProductionData[]>(`${API_BASE}/api/production-data`);
  return response.data;
};

export const fetchMachineProductionData = async (machineId: number): Promise<ProductionData[]> => {
  const response = await axios.get<ProductionData[]>(`${API_BASE}/api/machines/${machineId}/production-data`);
  return response.data;
};

export const postProductionData = async (data: Omit<ProductionData, 'id'>): Promise<ProductionData> => {
  const response = await axios.post<ProductionData>(`${API_BASE}/api/production-data`, data);
  return response.data;
};

// WebSocket connection for real-time updates
export const createWebSocketConnection = (
  onProductionDataUpdate: (data: ProductionData) => void,
  onError: (error: any) => void
) => {
  const ws = new WebSocket(`${WS_BASE}/ws/production-data`);

  ws.onmessage = (event) => {
    try {
      const data = JSON.parse(event.data) as ProductionData;
      onProductionDataUpdate(data);
    } catch (error) {
      onError(error);
    }
  };

  ws.onerror = (error) => {
    onError(error);
  };

  return ws;
};