﻿namespace PrediLiga.Domain.Entities
{
    public interface IEntity
    {
        long Id { get; set; }

        bool IsArchived { get; set; }
    }
}
