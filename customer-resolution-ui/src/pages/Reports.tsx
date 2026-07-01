import { useEffect, useState } from 'react';
import { Box, Typography, Paper, Grid } from '@mui/material';
import { ticketsApi } from '../api';
import type { DashboardStats } from '../models';

export default function Reports() {
  const [stats, setStats] = useState<DashboardStats | null>(null);

  useEffect(() => {
    ticketsApi.getDashboard().then((res) => setStats(res.data));
  }, []);

  if (!stats) return <Typography>Loading...</Typography>;

  const resolutionRate = stats.totalTickets > 0
    ? ((stats.resolvedTickets / stats.totalTickets) * 100).toFixed(1)
    : '0';

  return (
    <Box>
      <Typography variant="h4" sx={{ mb: 3 }}>Reports</Typography>

      <Grid container spacing={3}>
        <Grid size={{ xs: 12, md: 6 }}>
          <Paper sx={{ p: 3 }}>
            <Typography variant="h6" sx={{ mb: 2 }}>Ticket Summary</Typography>
            <Typography>Total Tickets: <strong>{stats.totalTickets}</strong></Typography>
            <Typography>Open: <strong>{stats.openTickets}</strong></Typography>
            <Typography>Resolved: <strong>{stats.resolvedTickets}</strong></Typography>
            <Typography>Pending (Assigned + InProgress): <strong>{stats.pendingTickets}</strong></Typography>
            <Typography>Created Today: <strong>{stats.todaysTickets}</strong></Typography>
          </Paper>
        </Grid>
        <Grid size={{ xs: 12, md: 6 }}>
          <Paper sx={{ p: 3 }}>
            <Typography variant="h6" sx={{ mb: 2 }}>Resolution Rate</Typography>
            <Typography variant="h2" color="success.main">{resolutionRate}%</Typography>
            <Typography color="text.secondary">
              {stats.resolvedTickets} of {stats.totalTickets} tickets resolved
            </Typography>
          </Paper>
        </Grid>
      </Grid>
    </Box>
  );
}
