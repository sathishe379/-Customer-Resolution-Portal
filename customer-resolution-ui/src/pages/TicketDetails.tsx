import { useEffect, useState } from 'react';
import { useParams, useNavigate } from 'react-router-dom';
import {
  Box,
  Typography,
  Paper,
  Chip,
  Divider,
  TextField,
  Button,
  List,
  ListItem,
  ListItemText,
  MenuItem,
  Grid,
  Alert,
} from '@mui/material';
import { ticketsApi, commentsApi, authApi } from '../api';
import type { Ticket, Comment, User } from '../models';
import { useAuth } from '../context/AuthContext';

const statuses = ['Open', 'Assigned', 'InProgress', 'Resolved', 'Closed'];

export default function TicketDetails() {
  const { id } = useParams<{ id: string }>();
  const { user } = useAuth();
  const navigate = useNavigate();
  const [ticket, setTicket] = useState<Ticket | null>(null);
  const [comments, setComments] = useState<Comment[]>([]);
  const [newComment, setNewComment] = useState('');
  const [newStatus, setNewStatus] = useState('');
  const [assignTo, setAssignTo] = useState<number | ''>('');
  const [engineers, setEngineers] = useState<User[]>([]);
  const [error, setError] = useState('');

  useEffect(() => {
    if (id) {
      ticketsApi.getById(Number(id)).then((res) => {
        setTicket(res.data);
        setNewStatus(res.data.status);
      });
      commentsApi.getByTicket(Number(id)).then((res) => setComments(res.data));
    }
    if (user?.role === 'Admin') {
      authApi.getUsers().then((res) =>
        setEngineers(res.data.filter((u) => u.role === 'SupportEngineer'))
      );
    }
  }, [id, user?.role]);

  const handleAddComment = async () => {
    if (!newComment.trim() || !id) return;
    try {
      const res = await commentsApi.create({ ticketId: Number(id), content: newComment });
      setComments([res.data, ...comments]);
      setNewComment('');
    } catch (err: any) {
      setError(err.response?.data?.message || 'Failed to add comment');
    }
  };

  const handleUpdateStatus = async () => {
    if (!id || !ticket) return;
    try {
      const update: any = { status: newStatus };
      if (assignTo) update.assignedTo = assignTo;
      const res = await ticketsApi.update(Number(id), update);
      setTicket(res.data);
      setError('');
    } catch (err: any) {
      setError(err.response?.data?.message || 'Failed to update ticket');
    }
  };

  if (!ticket) return <Typography>Loading...</Typography>;

  const statusColor = {
    Open: 'info',
    Assigned: 'warning',
    InProgress: 'secondary',
    Resolved: 'success',
    Closed: 'default',
  } as const;

  return (
    <Box>
      <Button variant="text" onClick={() => navigate('/tickets')} sx={{ mb: 2 }}>
        ← Back to Tickets
      </Button>

      {error && <Alert severity="error" sx={{ mb: 2 }}>{error}</Alert>}

      <Paper sx={{ p: 3, mb: 3 }}>
        <Box sx={{ display: 'flex', justifyContent: 'space-between', alignItems: 'flex-start' }}>
          <Typography variant="h5">{ticket.title}</Typography>
          <Chip
            label={ticket.status}
            color={statusColor[ticket.status as keyof typeof statusColor] || 'default'}
          />
        </Box>
        <Typography variant="body2" color="text.secondary" sx={{ mt: 1 }}>
          #{ticket.id} • Created by {ticket.creatorName} • {new Date(ticket.createdDate).toLocaleString()}
        </Typography>

        <Divider sx={{ my: 2 }} />

        <Grid container spacing={2}>
          <Grid size={{ xs: 12, md: 8 }}>
            <Typography variant="subtitle2">Description</Typography>
            <Typography variant="body1" sx={{ whiteSpace: 'pre-wrap' }}>{ticket.description}</Typography>
          </Grid>
          <Grid size={{ xs: 12, md: 4 }}>
            <Typography variant="subtitle2">Category</Typography>
            <Typography>{ticket.categoryName}</Typography>
            <Typography variant="subtitle2" sx={{ mt: 1 }}>Priority</Typography>
            <Typography>{ticket.priority}</Typography>
            <Typography variant="subtitle2" sx={{ mt: 1 }}>Assigned To</Typography>
            <Typography>{ticket.assigneeName || 'Unassigned'}</Typography>
            <Typography variant="subtitle2" sx={{ mt: 1 }}>Last Updated</Typography>
            <Typography>{new Date(ticket.updatedDate).toLocaleString()}</Typography>
          </Grid>
        </Grid>
      </Paper>

      {/* Status Update (Admin/Support) */}
      {(user?.role === 'Admin' || user?.role === 'SupportEngineer') && (
        <Paper sx={{ p: 3, mb: 3 }}>
          <Typography variant="h6" sx={{ mb: 2 }}>Update Ticket</Typography>
          <Box sx={{ display: 'flex', gap: 2, alignItems: 'center', flexWrap: 'wrap' }}>
            <TextField
              select
              size="small"
              label="Status"
              value={newStatus}
              onChange={(e) => setNewStatus(e.target.value)}
              sx={{ minWidth: 150 }}
            >
              {statuses.map((s) => (
                <MenuItem key={s} value={s}>{s}</MenuItem>
              ))}
            </TextField>
            {user?.role === 'Admin' && (
              <TextField
                select
                size="small"
                label="Assign To"
                value={assignTo}
                onChange={(e) => setAssignTo(Number(e.target.value))}
                sx={{ minWidth: 180 }}
              >
                <MenuItem value="">Unassigned</MenuItem>
                {engineers.map((e) => (
                  <MenuItem key={e.id} value={e.id}>{e.firstName} {e.lastName}</MenuItem>
                ))}
              </TextField>
            )}
            <Button variant="contained" onClick={handleUpdateStatus}>Update</Button>
          </Box>
        </Paper>
      )}

      {/* Comments Section */}
      <Paper sx={{ p: 3 }}>
        <Typography variant="h6" sx={{ mb: 2 }}>Comments</Typography>
        <Box sx={{ display: 'flex', gap: 2, mb: 2 }}>
          <TextField
            fullWidth
            size="small"
            placeholder="Add a comment..."
            value={newComment}
            onChange={(e) => setNewComment(e.target.value)}
            onKeyDown={(e) => e.key === 'Enter' && !e.shiftKey && handleAddComment()}
          />
          <Button variant="contained" onClick={handleAddComment}>Post</Button>
        </Box>
        <List>
          {comments.map((comment) => (
            <ListItem key={comment.id} sx={{ borderBottom: '1px solid #eee' }}>
              <ListItemText
                primary={comment.content}
                secondary={`${comment.authorName} • ${new Date(comment.createdDate).toLocaleString()}`}
              />
            </ListItem>
          ))}
          {comments.length === 0 && (
            <Typography color="text.secondary">No comments yet.</Typography>
          )}
        </List>
      </Paper>
    </Box>
  );
}
