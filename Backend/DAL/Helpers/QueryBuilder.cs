using System.Text;

namespace DAL.Helpers;

public class QueryBuilder
{
    StringBuilder _selectPart = new StringBuilder("Select ");
    StringBuilder _fromPart = new StringBuilder("From ");
    StringBuilder _wherePart = new StringBuilder("Where ");
    StringBuilder _orderByPart = new StringBuilder("Order By ");
    StringBuilder _limitPart = new StringBuilder("Limit ");
    StringBuilder _offsetPart = new StringBuilder("Offset ");
    
    public QueryBuilder Select(string select)
    {
        _selectPart.Append(select);
        return this;
    }
    
    public QueryBuilder From(string from)
    {
        _fromPart.Append(from);
        return this;
    }
    
    public QueryBuilder Where(string where)
    {
        _wherePart.Append(where);
        return this;
    }
    
    public QueryBuilder OrderBy(string orderBy)
    {
        _orderByPart.Append(orderBy);
        return this;
    }
    
    public QueryBuilder Limit(string limit)
    {
        _limitPart.Append(limit);
        return this;
    }
    
    public QueryBuilder Offset(string offset)
    {
        _offsetPart.Append(offset);
        return this;
    }
    
    public string Build()
    {
        StringBuilder query = new();
        query.Append(_selectPart);
        query.Append(_fromPart);
        query.Append(_wherePart);
        query.Append(_orderByPart);
        query.Append(_limitPart);
        query.Append(_offsetPart);
        return query.ToString();
    }
}