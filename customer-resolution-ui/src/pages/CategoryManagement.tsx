import { useEffect, useState } from 'react';
import {
  Box,
  Typography,
  Paper,
  Button,
  TextField,
  List,
  ListItem,
  ListItemText,
  Dialog,
  DialogTitle,
  DialogContent,
  DialogActions,
  Snackbar,
  Alert,
} from '@mui/material';
import { categoriesApi } from '../api';
import type { Category } from '../models';

export default function CategoryManagement() {
  const [categories, setCategories] = useState<Category[]>([]);
  const [dialogOpen, setDialogOpen] = useState(false);
  const [newCategory, setNewCategory] = useState('');
  const [snackbar, setSnackbar] = useState({ open: false, message: '', severity: 'success' as 'success' | 'error' });

  useEffect(() => {
    loadCategories();
  }, []);

  const loadCategories = () => {
    categoriesApi.getAll().then((res) => setCategories(res.data));
  };

  const handleCreate = async () => {
    if (!newCategory.trim()) return;
    try {
      await categoriesApi.create(newCategory.trim());
      setDialogOpen(false);
      setNewCategory('');
      loadCategories();
      setSnackbar({ open: true, message: 'Category created', severity: 'success' });
    } catch {
      setSnackbar({ open: true, message: 'Failed to create category', severity: 'error' });
    }
  };

  return (
    <Box>
      <Box sx={{ display: 'flex', justifyContent: 'space-between', mb: 3 }}>
        <Typography variant="h4" sx={{ mb: 0 }}>Categories</Typography>
        <Button variant="contained" onClick={() => setDialogOpen(true)}>
          Add Category
        </Button>
      </Box>

      <Paper sx={{ p: 2 }}>
        <List>
          {categories.map((cat) => (
            <ListItem key={cat.id} divider>
              <ListItemText primary={cat.categoryName} secondary={`ID: ${cat.id}`} />
            </ListItem>
          ))}
        </List>
      </Paper>

      <Dialog open={dialogOpen} onClose={() => setDialogOpen(false)}>
        <DialogTitle>Add New Category</DialogTitle>
        <DialogContent>
          <TextField
            fullWidth
            label="Category Name"
            value={newCategory}
            onChange={(e) => setNewCategory(e.target.value)}
            sx={{ mt: 2 }}
          />
        </DialogContent>
        <DialogActions>
          <Button onClick={() => setDialogOpen(false)}>Cancel</Button>
          <Button onClick={handleCreate} variant="contained">Create</Button>
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
