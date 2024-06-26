﻿using Catalog.API.Write.Commands;
using Catalog.Entities.DbSet;
using MediatR;
using Catalog.Infrastructure;

namespace Catalog.API.Write.Handlers
{
	public class CreateTypeCommandHandler(ICatalogRepository catalogRepository) : IRequestHandler<CreateTypeCommand, CatalogType?>
	{
        private readonly ICatalogRepository _catalogRepository = catalogRepository;

        public Task<CatalogType?> Handle(CreateTypeCommand request, CancellationToken cancellationToken)
            => _catalogRepository.CreateType(request.Name);
    }
}

