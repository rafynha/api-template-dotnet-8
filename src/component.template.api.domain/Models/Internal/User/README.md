# Query Interna: GetUserByIdInternalQuery

## Descrição
Esta query interna foi criada para buscar um usuário por ID e retornar o objeto `GetUserByIdInternalResponse` que representa o `UserDto` completo, permitindo que você faça o mapeamento para `UserDto` posteriormente.

## Uso

### Handler
O handler `GetUserByIdInternalQueryHandler` implementa a busca e retorna o `GetUserByIdInternalResponse` mapeado do repositório.

### Exemplo de Uso

```csharp
// No seu handler que precisa do UserDto completo
public async Task<SuaResponse> Handle(SeuCommand request, CancellationToken cancellationToken)
{
    // Buscar usando a query interna
    var userInternalResponse = await _mediator.Send(new GetUserByIdInternalQuery { Id = request.UserId }, cancellationToken);
    
    // Mapear para UserDto se necessário
    var userDto = _mapper.Map<UserDto>(userInternalResponse);
    
    // Ou usar diretamente as propriedades
    var response = _mapper.Map<SuaResponse>(userInternalResponse);
    
    // Ou usar propriedades específicas
    var customObject = new CustomObject
    {
        UserId = userInternalResponse.UserId,
        Username = userInternalResponse.Username,
        Email = userInternalResponse.Email,
        IsActive = userInternalResponse.IsActive,
        CreatedAt = userInternalResponse.CreatedAt,
        UpdatedAt = userInternalResponse.UpdatedAt,
        PasswordHash = userInternalResponse.PasswordHash // Disponível para operações internas
    };
    
    return response;
}
```

## Mapeamento Reverso para UserDto
Para converter `GetUserByIdInternalResponse` para `UserDto`:

```csharp
// Configure no seu handler
public override void ConfigureMappings(IMapperConfigurationExpression cfg)
{
    cfg.CreateMap<GetUserByIdInternalResponse, UserDto>()
        .ForMember(dest => dest.UserId, opt => opt.MapFrom(src => src.UserId))
        .ForMember(dest => dest.Username, opt => opt.MapFrom(src => src.Username))
        .ForMember(dest => dest.PasswordHash, opt => opt.MapFrom(src => src.PasswordHash))
        .ForMember(dest => dest.Email, opt => opt.MapFrom(src => src.Email))
        .ForMember(dest => dest.IsActive, opt => opt.MapFrom(src => src.IsActive))
        .ForMember(dest => dest.CreatedAt, opt => opt.MapFrom(src => src.CreatedAt))
        .ForMember(dest => dest.UpdatedAt, opt => opt.MapFrom(src => src.UpdatedAt));
}
```

## Validação
A query inclui validação via FluentValidation:
- O ID deve ser maior que zero

## Exceções
- `DataNotFoundException`: Quando o usuário não é encontrado
- `ValidationException`: Quando o ID é inválido

## Diferença da Query Externa
- **Query Externa** (`GetUserByIdQuery`): Retorna `GetUserByIdResponse` com dados limitados para exposição externa
- **Query Interna** (`GetUserByIdInternalQuery`): Retorna `GetUserByIdInternalResponse` que representa o UserDto completo para uso interno entre handlers

## Vantagens
1. **Encapsulamento**: Mantém a lógica de busca centralizada
2. **Reutilização**: Pode ser usada em qualquer handler que precise do UserDto completo
3. **Validação**: Inclui validação automática via pipeline
4. **Testabilidade**: Facilita testes unitários
5. **Consistência**: Padroniza a busca de usuários na aplicação