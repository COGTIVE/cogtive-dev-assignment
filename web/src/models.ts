export interface Machine {
  id: number;
  name: string;
  serialNumber: string;
  type: string;
  isActive: boolean;
}

export interface ProductionData {
  id: number;
  machineId: number;
  timestamp: string;
  efficiency: number;
  unitsProduced: number;
  downtime: number; // minutes
}