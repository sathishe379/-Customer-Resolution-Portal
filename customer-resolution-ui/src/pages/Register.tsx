import { useState } from 'react';
import { useNavigate, Link as RouterLink } from 'react-router-dom';
import { useForm } from 'react-hook-form';
import { Box, TextField, Button, Typography, Paper, Alert, Link } from '@mui/material';
import { authApi } from '../api';
import { useAuth } from '../context/AuthContext';
import type { RegisterRequest } from '../models';

export default function Register() {
  const { register, handleSubmit, formState: { errors } } = useForm<RegisterRequest>();
  const [error, setError] = useState('');
  const { login } = useAuth();
  const navigate = useNavigate();

  const onSubmit = async (data: RegisterRequest) => {
    try {
      setError('');
      const response = await authApi.register(data);
      login(response.data.token, response.data.user);
      navigate('/dashboard');
    } catch (err: any) {
      setError(err.response?.data?.message || 'Registration failed');
    }
  };

  return (
    <Box sx={{
      display: 'flex',
      justifyContent: 'center',
      alignItems: 'center',
      minHeight: '100vh',
      background: 'linear-gradient(135deg, #6366f1 0%, #a855f7 100%)',
      position: 'relative',
      '&::before': {
        content: '""',
        position: 'absolute',
        top: 0,
        left: 0,
        right: 0,
        bottom: 0,
        backgroundImage: `url("data:image/svg+xml,%3Csvg width='60' height='60' viewBox='0 0 60 60' xmlns='http://www.w3.org/2000/svg'%3E%3Cg fill='none' fill-rule='evenodd'%3E%3Cg fill='%23ffffff' fill-opacity='0.05'%3E%3Cpath d='M36 34v-4h-2v4h-4v2h4v4h2v-4h4v-2h-4zm0-30V0h-2v4h-4v2h4v4h2V6h4V4h-4zM6 34v-4H4v4H0v2h4v4h2v-4h4v-2H6zM6 4V0H4v4H0v2h4v4h2V6h4V4H6z'/%3E%3C/g%3E%3C/g%3E%3C/svg%3E")`,
      },
    }}>
      <Paper elevation={10} sx={{
        p: 4,
        maxWidth: 420,
        width: '100%',
        borderRadius: 3,
        backdropFilter: 'blur(10px)',
        backgroundColor: 'rgba(255, 255, 255, 0.95)',
        position: 'relative',
        zIndex: 1,
      }}>
        <Box sx={{ textAlign: 'center', mb: 2 }}>
          <Box
            component="img"
            src="/favicon.svg"
            alt="CRP Logo"
            sx={{ width: 50, height: 50, mb: 1 }}
          />
        </Box>
        <Typography variant="h5" sx={{ textAlign: 'center', mb: 1, fontWeight: 600, color: '#333' }}>
          Customer Resolution Portal
        </Typography>
        <Typography variant="body2" sx={{ textAlign: 'center', mb: 3, color: '#666' }}>
          Create your account
        </Typography>

        {error && <Alert severity="error" sx={{ mb: 2 }}>{error}</Alert>}

        <form onSubmit={handleSubmit(onSubmit)}>
          <TextField
            fullWidth
            label="First Name"
            margin="normal"
            {...register('firstName', { required: 'First name is required' })}
            error={!!errors.firstName}
            helperText={errors.firstName?.message}
          />
          <TextField
            fullWidth
            label="Last Name"
            margin="normal"
            {...register('lastName', { required: 'Last name is required' })}
            error={!!errors.lastName}
            helperText={errors.lastName?.message}
          />
          <TextField
            fullWidth
            label="Email"
            margin="normal"
            {...register('email', { required: 'Email is required', pattern: { value: /^\S+@\S+$/i, message: 'Invalid email' } })}
            error={!!errors.email}
            helperText={errors.email?.message}
          />
          <TextField
            fullWidth
            label="Password"
            type="password"
            margin="normal"
            {...register('password', { required: 'Password is required', minLength: { value: 6, message: 'Min 6 characters' } })}
            error={!!errors.password}
            helperText={errors.password?.message}
          />
          <Button type="submit" fullWidth variant="contained" sx={{
            mt: 3,
            mb: 1,
            py: 1.5,
            borderRadius: 2,
            background: 'linear-gradient(135deg, #6366f1 0%, #a855f7 100%)',
            fontWeight: 600,
            fontSize: '1rem',
            textTransform: 'none',
            boxShadow: '0 4px 15px rgba(99, 102, 241, 0.4)',
            '&:hover': {
              background: 'linear-gradient(135deg, #4f46e5 0%, #9333ea 100%)',
              boxShadow: '0 6px 20px rgba(99, 102, 241, 0.6)',
            },
          }}>
            Register
          </Button>
        </form>

        <Typography sx={{ textAlign: 'center', mt: 2 }}>
          Already have an account?{' '}
          <Link component={RouterLink} to="/login">Login</Link>
        </Typography>
      </Paper>
    </Box>
  );
}
