import React, { useEffect, useState, useCallback } from 'react';
import { fetchMachines, fetchMachineProductionData, createWebSocketConnection } from './api';
import { Machine, ProductionData } from './models';

const MachineList: React.FC = () => {
  const [machines, setMachines] = useState<Machine[]>([]);
  const [filteredMachines, setFilteredMachines] = useState<Machine[]>([]);
  const [selectedMachine, setSelectedMachine] = useState<number | null>(null);
  const [productionData, setProductionData] = useState<ProductionData[]>([]);
  const [loading, setLoading] = useState<boolean>(true);
  const [error, setError] = useState<string | null>(null);
  const [searchTerm, setSearchTerm] = useState<string>('');
  const [statusFilter, setStatusFilter] = useState<'all' | 'active' | 'inactive'>('all');
  const [sortConfig, setSortConfig] = useState<{ key: keyof Machine; direction: 'asc' | 'desc' } | null>(null);

  useEffect(() => {
    setLoading(true);
    fetchMachines()
      .then(data => {
        setMachines(data);
        setFilteredMachines(data);
        setLoading(false);
      })
      .catch((err) => {
        console.error(err);
        setError('Failed to load machines');
        setLoading(false);
      });
  }, []);

  useEffect(() => {
    let result = [...machines];

    // Apply search filter
    if (searchTerm) {
      const searchLower = searchTerm.toLowerCase();
      result = result.filter(machine => 
        machine.name.toLowerCase().includes(searchLower) ||
        machine.serialNumber.toLowerCase().includes(searchLower)
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
        if (a[sortConfig.key] < b[sortConfig.key]) {
          return sortConfig.direction === 'asc' ? -1 : 1;
        }
        if (a[sortConfig.key] > b[sortConfig.key]) {
          return sortConfig.direction === 'asc' ? 1 : -1;
        }
        return 0;
      });
    }

    setFilteredMachines(result);
  }, [machines, searchTerm, statusFilter, sortConfig]);

  const handleSort = (key: keyof Machine) => {
    setSortConfig(current => ({
      key,
      direction: current?.key === key && current.direction === 'asc' ? 'desc' : 'asc'
    }));
  };

  const handleMachineSelect = (machineId: number) => {
    setSelectedMachine(machineId);
    setLoading(true);
    fetchMachineProductionData(machineId)
      .then(data => {
        setProductionData(data);
        setLoading(false);
      })
      .catch((err) => {
        console.error(err);
        setError(`Failed to load production data for Machine #${machineId}`);
        setLoading(false);
      });
  };

  // Função para atualizar dados de produção em tempo real
  const handleProductionDataUpdate = useCallback((newData: ProductionData) => {
    setProductionData(currentData => {
      // Atualiza apenas se for a máquina selecionada
      if (selectedMachine === newData.machineId) {
        // Adiciona novo dado no início do array
        return [newData, ...currentData];
      }
      return currentData;
    });
  }, [selectedMachine]);

  // Configurar WebSocket
  useEffect(() => {
    const ws = createWebSocketConnection(
      handleProductionDataUpdate,
      (error) => console.error('WebSocket error:', error)
    );

    // Limpar conexão ao desmontar
    return () => {
      ws.close();
    };
  }, [handleProductionDataUpdate]);

  if (loading) {
    return <div className="loading-container">Loading...</div>;
  }

  if (error) {
    return (
      <div className="error-container">
        <div className="error-message">{error}</div>
        <button onClick={() => setError(null)}>Dismiss</button>
      </div>
    );
  }

  return (
    <div className="machines-container">
      <h2>Industrial Machines</h2>
      
      {/* Adicionar indicador de atualização em tempo real */}
      <div className="realtime-indicator">
        <span className="realtime-dot"></span>
        Atualização em tempo real
      </div>

      {/* Search and Filter Controls */}
      <div className="controls-container">
        <input
          type="text"
          placeholder="Search by name or serial number..."
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

      <table className="machines-table" border={1} cellPadding={5}>
        <thead>
          <tr>
            <th onClick={() => handleSort('id')} className="sortable">
              ID {sortConfig?.key === 'id' && (sortConfig.direction === 'asc' ? '↑' : '↓')}
            </th>
            <th onClick={() => handleSort('name')} className="sortable">
              Name {sortConfig?.key === 'name' && (sortConfig.direction === 'asc' ? '↑' : '↓')}
            </th>
            <th onClick={() => handleSort('serialNumber')} className="sortable">
              Serial Number {sortConfig?.key === 'serialNumber' && (sortConfig.direction === 'asc' ? '↑' : '↓')}
            </th>
            <th onClick={() => handleSort('type')} className="sortable">
              Type {sortConfig?.key === 'type' && (sortConfig.direction === 'asc' ? '↑' : '↓')}
            </th>
            <th onClick={() => handleSort('isActive')} className="sortable">
              Status {sortConfig?.key === 'isActive' && (sortConfig.direction === 'asc' ? '↑' : '↓')}
            </th>
            <th>Actions</th>
          </tr>
        </thead>
        <tbody>
          {filteredMachines.map((machine) => (
            <tr key={machine.id} className={machine.isActive ? 'active-machine' : 'inactive-machine'}>
              <td>{machine.id}</td>
              <td>{machine.name}</td>
              <td>{machine.serialNumber}</td>
              <td>{machine.type}</td>
              <td>
                <span className={`status-indicator ${machine.isActive ? 'status-active' : 'status-inactive'}`}>
                  {machine.isActive ? 'Active' : 'Inactive'}
                </span>
              </td>
              <td>
                <button onClick={() => handleMachineSelect(machine.id)}>
                  View Production Data
                </button>
              </td>
            </tr>
          ))}
        </tbody>
      </table>

      {selectedMachine && (
        <div className="production-data-container">
          <h3>Production Data for {machines.find(m => m.id === selectedMachine)?.name}</h3>
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
              {productionData.map((data) => (
                <tr key={data.id}>
                  <td>{new Date(data.timestamp).toLocaleString()}</td>
                  <td>{data.efficiency.toFixed(1)}</td>
                  <td>{data.unitsProduced}</td>
                  <td>{data.downtime}</td>
                </tr>
              ))}
            </tbody>
          </table>
        </div>
      )}

      {selectedMachine && productionData.length === 0 && (
        <div className="no-data-message">
          No production data available for this machine.
        </div>
      )}
    </div>
  );
};

export default MachineList;