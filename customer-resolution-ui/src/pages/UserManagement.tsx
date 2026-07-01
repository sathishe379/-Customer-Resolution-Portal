import { useEffect, useState } from 'react';
import {
  Box,
  Typography,
  Paper,
  Button,
  Dialog,
  DialogTitle,
  DialogContent,
  DialogActions,
  TextField,
  MenuItem,
  Snackbar,
  Alert,
} from '@mui/material';
import { DataGrid, type GridColDef } from '@mui/x-data-grid';
import { authApi } from '../api';
import type { User } from '../models';

const roles = ['Admin', 'SupportEngineer', 'Customer'];

export default function UserManagement() {
  const [users, setUsers] = useState<User[]>([]);
  const [selectedUser, setSelectedUser] = useState<User | null>(null);
  const [newRole, setNewRole] = useState('');
  const [dialogOpen, setDialogOpen] = useState(false);
  const [snackbar, setSnackbar] = useState({ open: false, message: '', severity: 'success' as 'success' | 'error' });

  useEffect(() => {
    loadUsers();
  }, []);

  const loadUsers = () => {
    authApi.getUsers().then((res) => setUsers(res.data));
  };

  const handleRoleChange = async () => {
    if (!selectedUser || !newRole) return;
    try {
      await authApi.updateUserRole(selectedUser.id, newRole);
      setDialogOpen(false);
      loadUsers();
      setSnackbar({ open: true, message: 'Role updated successfully', severity: 'success' });
    } catch {
      setSnackbar({ open: true, message: 'Failed to update role', severity: 'error' });
    }
  };

  const columns: GridColDef[] = [
    { field: 'id', headerName: 'ID', width: 70 },
    { field: 'firstName', headerName: 'First Name', width: 130 },
    { field: 'lastName', headerName: 'Last Name', width: 130 },
    { field: 'email', headerName: 'Email', flex: 1 },
    { field: 'role', headerName: 'Role', width: 150 },
    {
      field: 'createdDate',
      headerName: 'Created',
      width: 120,
      valueFormatter: (value: string) => new Date(value).toLocaleDateString(),
    },
    {
      field: 'actions',
      headerName: 'Actions',
      width: 130,
      renderCell: (params) => (
        <Button
          size="small"
          onClick={() => {
            setSelectedUser(params.row);
            setNewRole(params.row.role);
            setDialogOpen(true);
          }}
        >
          Change Role
        </Button>
      ),
    },
  ];

  return (
    <Box>
      <Typography variant="h4" sx={{ mb: 3 }}>User Management</Typography>
      <Paper sx={{ p: 2 }}>
        <DataGrid
          rows={users}
          columns={columns}
          pageSizeOptions={[10, 25]}
          initialState={{ pagination: { paginationModel: { pageSize: 10 } } }}
          sx={{ height: 500 }}
        />
      </Paper>

      <Dialog open={dialogOpen} onClose={() => setDialogOpen(false)}>
        <DialogTitle>Change Role for {selectedUser?.firstName} {selectedUser?.lastName}</DialogTitle>
        <DialogContent>
          <TextField
            select
            fullWidth
            label="Role"
            value={newRole}
            onChange={(e) => setNewRole(e.target.value)}
            sx={{ mt: 2 }}
          >
            {roles.map((r) => (
              <MenuItem key={r} value={r}>{r}</MenuItem>
            ))}
          </TextField>
        </DialogContent>
        <DialogActions>
          <Button onClick={() => setDialogOpen(false)}>Cancel</Button>
          <Button onClick={handleRoleChange} variant="contained">Save</Button>
        </DialogActions>
      </Dialog>

      <Snackbar
        open={snackbar.open}
        autoHideDuration={3000}
        onClose={() => setSnackbar({ ...snackbar, open: false })}
      >
        <Alert severity={snackbar.severity}>{snackbar.message}</Alert>
      </Snackbar>
    </Box>
  );
}
