import { useState, useEffect } from 'react';
import { useNavigate } from 'react-router-dom';
import { useForm, Controller } from 'react-hook-form';
import {
  Box,
  TextField,
  Button,
  Typography,
  Paper,
  MenuItem,
  Alert,
} from '@mui/material';
import { ticketsApi, categoriesApi } from '../api';
import type { CreateTicketRequest, Category } from '../models';

const priorities = ['Low', 'Medium', 'High', 'Critical'];

export default function CreateTicket() {
  const { control, handleSubmit, formState: { errors } } = useForm<CreateTicketRequest>({
    defaultValues: { priority: 'Medium' },
  });
  const [categories, setCategories] = useState<Category[]>([]);
  const [error, setError] = useState('');
  const navigate = useNavigate();

  useEffect(() => {
    categoriesApi.getAll().then((res) => setCategories(res.data));
  }, []);

  const onSubmit = async (data: CreateTicketRequest) => {
    try {
      setError('');
      await ticketsApi.create(data);
      navigate('/tickets');
    } catch (err: any) {
      setError(err.response?.data?.message || 'Failed to create ticket');
    }
  };

  return (
    <Box sx={{ maxWidth: 600, mx: 'auto' }}>
      <Typography variant="h4" sx={{ mb: 3 }}>Create Ticket</Typography>
      <Paper sx={{ p: 3 }}>
        {error && <Alert severity="error" sx={{ mb: 2 }}>{error}</Alert>}

        <form onSubmit={handleSubmit(onSubmit)}>
          <Controller
            name="title"
            control={control}
            rules={{ required: 'Title is required' }}
            render={({ field }) => (
              <TextField
                {...field}
                fullWidth
                label="Title"
                margin="normal"
                error={!!errors.title}
                helperText={errors.title?.message}
              />
            )}
          />
          <Controller
            name="description"
            control={control}
            rules={{ required: 'Description is required' }}
            render={({ field }) => (
              <TextField
                {...field}
                fullWidth
                label="Description"
                margin="normal"
                multiline
                rows={4}
                error={!!errors.description}
                helperText={errors.description?.message}
              />
            )}
          />
          <Controller
            name="categoryId"
            control={control}
            rules={{ required: 'Category is required' }}
            render={({ field }) => (
              <TextField
                {...field}
                fullWidth
                select
                label="Category"
                margin="normal"
                error={!!errors.categoryId}
                helperText={errors.categoryId?.message}
              >
                {categories.map((c) => (
                  <MenuItem key={c.id} value={c.id}>{c.categoryName}</MenuItem>
                ))}
              </TextField>
            )}
          />
          <Controller
            name="priority"
            control={control}
            render={({ field }) => (
              <TextField
                {...field}
                fullWidth
                select
                label="Priority"
                margin="normal"
              >
                {priorities.map((p) => (
                  <MenuItem key={p} value={p}>{p}</MenuItem>
                ))}
              </TextField>
            )}
          />
          <Box sx={{ mt: 2, display: 'flex', gap: 2 }}>
            <Button type="submit" variant="contained">Create</Button>
            <Button variant="outlined" onClick={() => navigate('/tickets')}>Cancel</Button>
          </Box>
        </form>
      </Paper>
    </Box>
  );
}
