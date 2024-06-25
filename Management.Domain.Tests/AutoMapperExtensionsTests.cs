using AutoMapper;

using FluentAssertions;

using global::Managemt.Api.Extensions;

namespace Management.Domain.Tests;

[Trait("Category", "UnitTests")]
public class AutoMapperExtensionsTests
{
    private readonly IMapper _mapper;

    public AutoMapperExtensionsTests()
    {
        var config = new MapperConfiguration(cfg =>
        {
            cfg.AddMaps(typeof(AutoMapperExtensionsTests).Assembly);
            cfg.AllowNullCollections = true;
            cfg.AllowNullDestinationValues = true;
        });
        _mapper = config.CreateMapper();
    }

    [Fact]
    public void MapTo_HandlesCircularReferenceInNestedObjects_Success()
    {
        // Arrange
        var parent = new Parent
        {
            Id = 1,
            Name = "Parent",
            Child = new Child
            {
                Id = 2,
                Name = "Child",
                Parent = null
            }
        };

        // Act
        var mappedParent = parent.MapTo<Parent>();

        // Assert
        mappedParent.Should().NotBeNull();
        mappedParent.Child.Should().NotBeNull();
        mappedParent.Child.Parent.Should().BeNull(); // Circular reference is handled
        parent.Id.Should().Be(mappedParent.Id);
        parent.Name.Should().Be(mappedParent.Name);
        parent.Child.Id.Should().Be(mappedParent.Child.Id);
        parent.Child.Name.Should().Be(mappedParent.Child.Name);
    }

    [Fact]
    public void MapTo_HandlesCircularReferenceInNestedObjects_WithNullReference()
    {
        // Arrange
        var parent = new Parent
        {
            Id = 1,
            Name = "Parent",
            Child = null
        };

        // Act
        var mappedParent = parent.MapTo<Parent>();

        // Assert
        mappedParent.Should().NotBeNull();
        parent.Id.Should().Be(mappedParent.Id);
        parent.Name.Should().Be(mappedParent.Name);
        mappedParent.Child.Should().BeNull(); // Null reference is handled
    }

    [Fact]
    public void MapTo_HandlesCircularReferenceInNestedObjects_WithEmptyCollection()
    {
        // Arrange
        var parent = new Parent
        {
            Id = 1,
            Name = "Parent",
            Children = new List<Child>()
        };

        // Act
        var mappedParent = parent.MapTo<Parent>();

        // Assert
        mappedParent.Should().NotBeNull();
        parent.Id.Should().Be(mappedParent.Id);
        parent.Name.Should().Be(mappedParent.Name);
        mappedParent.Children.Should().NotBeNull();
        mappedParent.Children.Should().BeEmpty(); // Empty collection is handled
    }

    [Fact]
    public void MapTo_HandlesCircularReferenceInNestedObjects_WithNullCollection()
    {
        // Arrange
        var parent = new Parent
        {
            Id = 1,
            Name = "Parent",
            Children = null
        };

        // Act
        var mappedParent = parent.MapTo<Parent>();

        // Assert
        mappedParent.Should().NotBeNull();
        parent.Id.Should().Be(mappedParent.Id);
        parent.Name.Should().Be(mappedParent.Name);
        mappedParent.Children.Should().BeNull(); // Null collection is handled
    }

    public class Parent
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public Child Child { get; set; }
        public List<Child> Children { get; set; }
    }

    public class Child
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public Parent Parent { get; set; }
    }
}