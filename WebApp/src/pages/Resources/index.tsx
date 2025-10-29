import { useState } from 'react';
import { Container, Typography } from '@mui/material';
import type { SystemResource } from '../../interfaces';
import { useSystemResources } from '../../hooks';
import {
  SystemResourceForm,
  SystemResourcesTable,
  SystemResourceEditionModal,
} from '../../components';

export default function Resources() {
  const {
    fetchSystemResources,
    addSystemResource,
    editSystemResource,
    removeSystemResource,
    pagination,
  } = useSystemResources();

  const [editingResource, setEditingResource] = useState<SystemResource | null>(
    null
  );
  const [open, setOpen] = useState(false);

  async function handleCreate(resource: SystemResource) {
    try {
      await addSystemResource(resource);
      alert('✅ Recurso criado com sucesso!');
      fetchSystemResources(pagination.page, pagination.pageSize); // atualiza tabela
    } catch (err) {
      console.error(err);
      alert('❌ Erro ao criar recurso');
    }
  }

  async function handleUpdate(resource: SystemResource) {
    if (!editingResource) return;
    try {
      await editSystemResource({ ...editingResource, ...resource });
      alert('✅ Recurso atualizado com sucesso!');
      setOpen(false);
      fetchSystemResources(pagination.page, pagination.pageSize);
    } catch (err) {
      console.error(err);
      alert('❌ Erro ao atualizar recurso');
    }
  }

  async function handleDelete(id: number) {
    const confirmDelete = confirm(
      'Tem certeza que deseja excluir este recurso?'
    );
    if (!confirmDelete) return;

    try {
      await removeSystemResource(id.toString());
      alert('🗑️ Recurso excluído com sucesso!');
      fetchSystemResources(pagination.page, pagination.pageSize);
    } catch (err) {
      console.error(err);
      alert('❌ Erro ao excluir recurso');
    }
  }

  function handleOpenEditionModal(resource: SystemResource) {
    setEditingResource(resource);
    setOpen(true);
  }

  return (
    <Container
      sx={{
        mt: 4,
        display: 'flex',
        flexDirection: 'column',
        alignItems: 'center',
        justifyContent: 'center',
        textAlign: 'center',
      }}
    >
      <Typography variant="h5" gutterBottom>
        Gerenciamento de Recursos
      </Typography>

      <SystemResourceForm onSubmit={handleCreate} />

      <SystemResourcesTable
        onEdit={handleOpenEditionModal}
        onDelete={handleDelete}
      />

      <SystemResourceEditionModal
        open={open}
        resource={editingResource}
        onClose={() => setOpen(false)}
        onSubmit={handleUpdate}
      />
    </Container>
  );
}
