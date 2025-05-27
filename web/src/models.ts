export interface Machine {
  id: number;
  name: string;
  serialNumber: string;
  type: string;
  installationDate: string;
  isActive: boolean;
  description?: string;
  productionData?: ProductionData[];
}

export interface ProductionData {
  id: number;
  machineId: number;
  timestamp: string;
  efficiency: number;
  unitsProduced: number;
  downtime: number; // minutes
}