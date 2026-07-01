import { useEffect, useState } from 'react';
import { useNavigate } from 'react-router-dom';
import {
  Box,
  Typography,
  Button,
  TextField,
  MenuItem,
  Stack,
  Chip,
} from '@mui/material';
import { DataGrid, type GridColDef } from '@mui/x-data-grid';
import { ticketsApi, categoriesApi } from '../api';
import type { Ticket, Category, TicketFilter } from '../models';

const statusOptions = ['', 'Open', 'Assigned', 'InProgress', 'Resolved', 'Closed'];
const priorityOptions = ['', 'Low', 'Medium', 'High', 'Critical'];

export default function TicketList() {
  const [tickets, setTickets] = useState<Ticket[]>([]);
  const [categories, setCategories] = useState<Category[]>([]);
  const [filter, setFilter] = useState<TicketFilter>({});
  const navigate = useNavigate();

  useEffect(() => {
    loadTickets();
    categoriesApi.getAll().then((res) => setCategories(res.data));
  }, []);

  const loadTickets = (f?: TicketFilter) => {
    ticketsApi.getAll(f || filter).then((res) => setTickets(res.data));
  };

  const handleSearch = () => loadTickets();

  const handleClear = () => {
    setFilter({});
    loadTickets({});
  };

  const columns: GridColDef[] = [
    { field: 'id', headerName: 'ID', width: 70 },
    { field: 'title', headerName: 'Title', flex: 1 },
    { field: 'categoryName', headerName: 'Category', width: 130 },
    {
      field: 'priority',
      headerName: 'Priority',
      width: 100,
      renderCell: (params) => (
        <Typography
          variant="body2"
          sx={{
            color:
              params.value === 'Critical' ? 'error.main' :
              params.value === 'High' ? 'warning.main' :
              params.value === 'Medium' ? 'info.main' : 'success.main',
            fontWeight: 'bold',
          }}
        >
          {params.value}
        </Typography>
      ),
    },
    {
      field: 'status',
      headerName: 'Status',
      width: 130,
      headerAlign: 'center',
      align: 'center',
      renderCell: (params) => {
        const colorMap: Record<string, 'default' | 'primary' | 'info' | 'warning' | 'success' | 'error'> = {
          Open: 'info',
          Assigned: 'primary',
          InProgress: 'warning',
          Resolved: 'success',
          Closed: 'default',
        };
        return (
          <Chip
            label={params.value}
            size="small"
            color={colorMap[params.value as string] || 'default'}
            variant="outlined"
          />
        );
      },
    },
    { field: 'creatorName', headerName: 'Created By', width: 140 },
    {
      field: 'assigneeName',
      headerName: 'Assigned To',
      width: 140,
      headerAlign: 'center',
      align: 'center',
      renderCell: (params) => (
        <Typography variant="body2" sx={{ color: params.value ? 'text.primary' : 'text.secondary', fontStyle: params.value ? 'normal' : 'italic' }}>
          {params.value || 'Unassigned'}
        </Typography>
      ),
    },
    {
      field: 'createdDate',
      headerName: 'Created',
      width: 120,
      valueFormatter: (value: string) => new Date(value).toLocaleDateString(),
    },
  ];

  return (
    <Box>
      <Box sx={{ display: 'flex', justifyContent: 'space-between', mb: 2 }}>
        <Typography variant="h4">Tickets</Typography>
        <Button variant="contained" onClick={() => navigate('/tickets/create')}>
          Create Ticket
        </Button>
      </Box>

      <Stack direction="row" spacing={2} sx={{ mb: 2, flexWrap: 'wrap' }}>
        <TextField
          size="small"
          label="Search Title"
          value={filter.title || ''}
          onChange={(e) => setFilter({ ...filter, title: e.target.value })}
        />
        <TextField
          size="small"
          select
          label="Priority"
          value={filter.priority || ''}
          onChange={(e) => setFilter({ ...filter, priority: e.target.value })}
          sx={{ minWidth: 120 }}
        >
          {priorityOptions.map((p) => (
            <MenuItem key={p} value={p}>{p || 'All'}</MenuItem>
          ))}
        </TextField>
        <TextField
          size="small"
          select
          label="Status"
          value={filter.status || ''}
          onChange={(e) => setFilter({ ...filter, status: e.target.value })}
          sx={{ minWidth: 120 }}
        >
          {statusOptions.map((s) => (
            <MenuItem key={s} value={s}>{s || 'All'}</MenuItem>
          ))}
        </TextField>
        <TextField
          size="small"
          select
          label="Category"
          value={filter.categoryId || ''}
          onChange={(e) => setFilter({ ...filter, categoryId: e.target.value ? Number(e.target.value) : undefined })}
          sx={{ minWidth: 130 }}
        >
          <MenuItem value="">All</MenuItem>
          {categories.map((c) => (
            <MenuItem key={c.id} value={c.id}>{c.categoryName}</MenuItem>
          ))}
        </TextField>
        <Button variant="outlined" onClick={handleSearch}>Search</Button>
        <Button variant="text" onClick={handleClear}>Clear</Button>
      </Stack>

      <DataGrid
        rows={tickets}
        columns={columns}
        pageSizeOptions={[10, 25, 50]}
        initialState={{ pagination: { paginationModel: { pageSize: 10 } } }}
        onRowClick={(params) => navigate(`/tickets/${params.id}`)}
        sx={{
          cursor: 'pointer',
          height: 500,
          '& .MuiDataGrid-cell': {
            display: 'flex',
            alignItems: 'center',
          },
        }}
      />
    </Box>
  );
}
