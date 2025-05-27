import React, { useEffect, useState, useCallback, useMemo } from 'react';
import { fetchMachines, fetchMachineProductionData, createWebSocketConnection } from './api';
import { Machine, ProductionData } from './models';

// Custom hook para gerenciar o estado das máquinas
const useMachines = () => {
  const [machines, setMachines] = useState<Machine[]>([]);
  const [loading, setLoading] = useState<boolean>(true);
  const [error, setError] = useState<string | null>(null);

  useEffect(() => {
    const loadMachines = async () => {
      try {
        setLoading(true);
        const data = await fetchMachines();
        setMachines(data);
      } catch (err) {
        console.error(err);
        setError('Failed to load machines');
      } finally {
        setLoading(false);
      }
    };

    loadMachines();
  }, []);

  return { machines, setMachines, loading, error, setError };
};

// Custom hook para gerenciar filtros e ordenação
const useMachineFilters = (machines: Machine[]) => {
  const [searchTerm, setSearchTerm] = useState<string>('');
  const [statusFilter, setStatusFilter] = useState<'all' | 'active' | 'inactive'>('all');
  const [sortConfig, setSortConfig] = useState<{ key: keyof Machine; direction: 'asc' | 'desc' } | null>(null);

  const filteredMachines = useMemo(() => {
    let result = [...machines];

    // Apply search filter
    if (searchTerm) {
      const searchLower = searchTerm.toLowerCase();
      result = result.filter(machine => 
        machine.name?.toLowerCase().includes(searchLower) ||
        machine.serialNumber?.toLowerCase().includes(searchLower) ||
        machine.type?.toLowerCase().includes(searchLower)
      );
    }

    // Apply status filter
    if (statusFilter !== 'all') {
      result = result.filter(machine => 
        statusFilter === 'active' ? machine.isActive : !machine.isActive
      );
    }

    // Apply sorting
    if (sortConfig) {
      result.sort((a, b) => {
        const aValue = a[sortConfig.key];
        const bValue = b[sortConfig.key];
        
        if (aValue === undefined || bValue === undefined) return 0;
        
        if (aValue < bValue) {
          return sortConfig.direction === 'asc' ? -1 : 1;
        }
        if (aValue > bValue) {
          return sortConfig.direction === 'asc' ? 1 : -1;
        }
        return 0;
      });
    }

    return result;
  }, [machines, searchTerm, statusFilter, sortConfig]);

  const handleSort = useCallback((key: keyof Machine) => {
    setSortConfig(current => ({
      key,
      direction: current?.key === key && current.direction === 'asc' ? 'desc' : 'asc'
    }));
  }, []);

  return {
    searchTerm,
    setSearchTerm,
    statusFilter,
    setStatusFilter,
    sortConfig,
    handleSort,
    filteredMachines
  };
};

// Custom hook para gerenciar dados de produção
const useProductionData = (selectedMachine: number | null) => {
  const [productionData, setProductionData] = useState<ProductionData[]>([]);
  const [loading, setLoading] = useState<boolean>(false);
  const [error, setError] = useState<string | null>(null);

  useEffect(() => {
    if (!selectedMachine) {
      setProductionData([]);
      return;
    }

    const loadProductionData = async () => {
      try {
        setLoading(true);
        const data = await fetchMachineProductionData(selectedMachine);
        setProductionData(data);
      } catch (err) {
        console.error(err);
        setError(`Failed to load production data for Machine #${selectedMachine}`);
      } finally {
        setLoading(false);
      }
    };

    loadProductionData();
  }, [selectedMachine]);

  return { productionData, setProductionData, loading, error, setError };
};

// Componente para a tabela de máquinas
const MachinesTable = React.memo(({ 
  machines, 
  onSort, 
  sortConfig, 
  onMachineSelect,
  isUpdating 
}: { 
  machines: Machine[]; 
  onSort: (key: keyof Machine) => void; 
  sortConfig: { key: keyof Machine; direction: 'asc' | 'desc' } | null;
  onMachineSelect: (id: number) => void;
  isUpdating: boolean;
}) => (
  <div className="table-container">
    {isUpdating && (
      <div className="updating-indicator">
        <span className="updating-dot"></span>
        Atualizando dados...
      </div>
    )}
    <table className="machines-table" border={1} cellPadding={5}>
      <thead>
        <tr>
          <th onClick={() => onSort('id')} className="sortable">
            ID {sortConfig?.key === 'id' && (sortConfig.direction === 'asc' ? '↑' : '↓')}
          </th>
          <th onClick={() => onSort('name')} className="sortable">
            Name {sortConfig?.key === 'name' && (sortConfig.direction === 'asc' ? '↑' : '↓')}
          </th>
          <th onClick={() => onSort('serialNumber')} className="sortable">
            Serial Number {sortConfig?.key === 'serialNumber' && (sortConfig.direction === 'asc' ? '↑' : '↓')}
          </th>
          <th onClick={() => onSort('type')} className="sortable">
            Type {sortConfig?.key === 'type' && (sortConfig.direction === 'asc' ? '↑' : '↓')}
          </th>
          <th onClick={() => onSort('installationDate')} className="sortable">
            Installation Date {sortConfig?.key === 'installationDate' && (sortConfig.direction === 'asc' ? '↑' : '↓')}
          </th>
          <th onClick={() => onSort('isActive')} className="sortable">
            Status {sortConfig?.key === 'isActive' && (sortConfig.direction === 'asc' ? '↑' : '↓')}
          </th>
          <th>Actions</th>
        </tr>
      </thead>
      <tbody>
        {machines.map((machine) => (
          <tr key={machine.id} className={machine.isActive ? 'active-machine' : 'inactive-machine'}>
            <td>{machine.id}</td>
            <td>{machine.name}</td>
            <td>{machine.serialNumber}</td>
            <td>{machine.type}</td>
            <td>{new Date(machine.installationDate).toLocaleDateString()}</td>
            <td>
              <span className={`status-indicator ${machine.isActive ? 'status-active' : 'status-inactive'}`}>
                {machine.isActive ? 'Active' : 'Inactive'}
              </span>
            </td>
            <td>
              <button onClick={() => onMachineSelect(machine.id)}>
                View Production Data
              </button>
            </td>
          </tr>
        ))}
      </tbody>
    </table>
  </div>
));

// Componente para a tabela de dados de produção
const ProductionDataTable = React.memo(({ 
  data, 
  machineName 
}: { 
  data: ProductionData[]; 
  machineName: string;
}) => (
  <div className="production-data-container">
    <h3>Production Data for {machineName}</h3>
    <table className="production-table" border={1} cellPadding={5}>
      <thead>
        <tr>
          <th>Timestamp</th>
          <th>Efficiency (%)</th>
          <th>Units Produced</th>
          <th>Downtime (min)</th>
        </tr>
      </thead>
      <tbody>
        {data.map((item) => (
          <tr key={item.id}>
            <td>{new Date(item.timestamp).toLocaleString()}</td>
            <td>{item.efficiency.toFixed(1)}</td>
            <td>{item.unitsProduced}</td>
            <td>{item.downtime}</td>
          </tr>
        ))}
      </tbody>
    </table>
  </div>
));

const MachineList: React.FC = () => {
  const { machines, setMachines, loading: machinesLoading, error: machinesError, setError: setMachinesError } = useMachines();
  const [selectedMachine, setSelectedMachine] = useState<number | null>(null);
  const [isUpdating, setIsUpdating] = useState<boolean>(false);
  const { 
    searchTerm, 
    setSearchTerm, 
    statusFilter, 
    setStatusFilter, 
    sortConfig, 
    handleSort, 
    filteredMachines 
  } = useMachineFilters(machines);
  const { 
    productionData, 
    setProductionData, 
    loading: productionLoading, 
    error: productionError,
    setError: setProductionError 
  } = useProductionData(selectedMachine);

  const handleMachineSelect = useCallback((machineId: number) => {
    setSelectedMachine(machineId);
  }, []);

  const handleProductionDataUpdate = useCallback((data: ProductionData) => {
    // Validação dos dados recebidos
    if (!data || typeof data.machineId !== 'number') {
      console.error('Dados inválidos recebidos do WebSocket:', data);
      return;
    }

    console.log('Recebendo atualização em tempo real:', {
      machineId: data.machineId,
      timestamp: new Date(data.timestamp).toLocaleString(),
      efficiency: data.efficiency,
      unitsProduced: data.unitsProduced
    });

    // Atualiza imediatamente os dados da máquina
    setMachines(prevMachines => {
      const updatedMachines = prevMachines.map(machine => {
        if (machine.id === data.machineId) {
          const updatedProductionData = [
            data,
            ...(machine.productionData || []).slice(0, 99)
          ];
          return {
            ...machine,
            productionData: updatedProductionData
          };
        }
        return machine;
      });

      // Se a máquina não existe, busca ela da API imediatamente
      if (!updatedMachines.some(m => m.id === data.machineId)) {
        console.log('Máquina não encontrada, buscando da API:', data.machineId);
        fetch(`http://localhost:5000/api/machines/${data.machineId}`)
          .then(async res => {
            if (!res.ok) {
              throw new Error(`HTTP error! status: ${res.status}`);
            }
            const newMachine = await res.json();
            setMachines(prev => {
              const machineExists = prev.some(m => m.id === newMachine.id);
              if (!machineExists) {
                console.log('Nova máquina adicionada:', newMachine.id);
                return [...prev, { ...newMachine, productionData: [data] }];
              }
              return prev;
            });
          })
          .catch(error => {
            console.error('Erro ao buscar nova máquina:', error);
            // Não tenta adicionar a máquina se houver erro
          });
      }

      return updatedMachines;
    });

    // Atualiza imediatamente os dados de produção se for a máquina selecionada
    if (selectedMachine === data.machineId) {
      console.log('Atualizando dados de produção para máquina selecionada:', selectedMachine);
      setProductionData(prev => [data, ...prev.slice(0, 99)]);
    }
  }, [selectedMachine, setMachines, setProductionData]);

  // Configurar WebSocket com reconexão automática e validação de dados
  useEffect(() => {
    let ws: WebSocket | null = null;
    let reconnectTimeout: NodeJS.Timeout;

    const connectWebSocket = () => {
      ws = createWebSocketConnection(
        (data) => {
          try {
            // Validação do formato dos dados
            if (data && typeof data === 'object' && 'machineId' in data) {
              handleProductionDataUpdate(data as ProductionData);
            } else {
              console.error('Formato de dados inválido recebido do WebSocket:', data);
            }
          } catch (error) {
            console.error('Erro ao processar dados do WebSocket:', error);
          }
        },
        (error) => {
          console.error('WebSocket error:', error);
          // Tenta reconectar após 3 segundos
          reconnectTimeout = setTimeout(connectWebSocket, 3000);
        }
      );
    };

    connectWebSocket();

    return () => {
      if (ws) {
        ws.close();
      }
      if (reconnectTimeout) {
        clearTimeout(reconnectTimeout);
      }
    };
  }, [handleProductionDataUpdate]);

  if (machinesLoading) {
    return <div className="loading-container">Loading...</div>;
  }

  if (machinesError) {
    return (
      <div className="error-container">
        <div className="error-message">{machinesError}</div>
        <button onClick={() => setMachinesError(null)}>Dismiss</button>
      </div>
    );
  }

  const selectedMachineName = machines.find(m => m.id === selectedMachine)?.name;

  return (
    <div className="machines-container">
      <h2>Industrial Machines</h2>
      
      <div className="realtime-indicator">
        <span className="realtime-dot"></span>
        Atualização em tempo real
      </div>

      <div className="controls-container">
        <input
          type="text"
          placeholder="Search by name, serial number or type..."
          value={searchTerm}
          onChange={(e) => setSearchTerm(e.target.value)}
          className="search-input"
        />
        
        <select
          value={statusFilter}
          onChange={(e) => setStatusFilter(e.target.value as 'all' | 'active' | 'inactive')}
          className="status-filter"
        >
          <option value="all">All Status</option>
          <option value="active">Active Only</option>
          <option value="inactive">Inactive Only</option>
        </select>
      </div>

      <MachinesTable
        machines={filteredMachines}
        onSort={handleSort}
        sortConfig={sortConfig}
        onMachineSelect={handleMachineSelect}
        isUpdating={isUpdating}
      />

      {selectedMachine && (
        <>
          {productionLoading ? (
            <div className="loading-container">Loading production data...</div>
          ) : productionError ? (
            <div className="error-container">
              <div className="error-message">{productionError}</div>
              <button onClick={() => setProductionError(null)}>Dismiss</button>
            </div>
          ) : productionData.length > 0 ? (
            <ProductionDataTable
              data={productionData}
              machineName={selectedMachineName || ''}
            />
          ) : (
            <div className="no-data-message">
              No production data available for this machine.
            </div>
          )}
        </>
      )}
    </div>
  );
};

export default React.memo(MachineList);