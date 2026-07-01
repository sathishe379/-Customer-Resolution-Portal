import { useEffect, useState } from 'react';
import { Grid, Card, CardContent, Typography, Box } from '@mui/material';
import {
  ConfirmationNumber,
  CheckCircle,
  HourglassEmpty,
  Today,
  FolderOpen,
} from '@mui/icons-material';
import { ticketsApi } from '../api';
import type { DashboardStats } from '../models';

export default function Dashboard() {
  const [stats, setStats] = useState<DashboardStats | null>(null);

  useEffect(() => {
    ticketsApi.getDashboard().then((res) => setStats(res.data));
  }, []);

  if (!stats) return <Typography>Loading...</Typography>;

  const cards = [
    { title: 'Total Tickets', value: stats.totalTickets, icon: <ConfirmationNumber fontSize="large" />, color: '#1976d2' },
    { title: 'Open Tickets', value: stats.openTickets, icon: <FolderOpen fontSize="large" />, color: '#ed6c02' },
    { title: 'Resolved Tickets', value: stats.resolvedTickets, icon: <CheckCircle fontSize="large" />, color: '#2e7d32' },
    { title: 'Pending Tickets', value: stats.pendingTickets, icon: <HourglassEmpty fontSize="large" />, color: '#9c27b0' },
    { title: "Today's Tickets", value: stats.todaysTickets, icon: <Today fontSize="large" />, color: '#0288d1' },
  ];

  return (
    <Box>
      <Typography variant="h4" sx={{ mb: 3 }}>Dashboard</Typography>
      <Grid container spacing={3}>
        {cards.map((card) => (
          <Grid size={{ xs: 12, sm: 6, md: 4 }} key={card.title}>
            <Card sx={{ borderLeft: `4px solid ${card.color}` }}>
              <CardContent sx={{ display: 'flex', alignItems: 'center', gap: 2 }}>
                <Box sx={{ color: card.color }}>{card.icon}</Box>
                <Box>
                  <Typography variant="h4">{card.value}</Typography>
                  <Typography color="text.secondary">{card.title}</Typography>
                </Box>
              </CardContent>
            </Card>
          </Grid>
        ))}
      </Grid>
    </Box>
  );
}
