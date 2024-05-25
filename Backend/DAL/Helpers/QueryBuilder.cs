using System.Text;

namespace DAL.Helpers;

public class QueryBuilder
{
    StringBuilder _selectPart = new("SELECT ");
    StringBuilder _fromPart = new("FROM ");
    StringBuilder _wherePart = new("Where ");
    StringBuilder _groupByPart = new("Group By ");
    StringBuilder _orderByPart = new("Order By ");
    StringBuilder _havingPart = new("Having ");
    StringBuilder _limitPart = new("Limit ");
    StringBuilder _offsetPart = new("Offset ");
    
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
    
    public QueryBuilder GroupBy(string groupBy)
    {
        _groupByPart.Append(groupBy);
        return this;
    }
    
    public QueryBuilder Having(string having)
    {
        _havingPart.Append(having);
        return this;
    }
    
    public string Build()
    {
        StringBuilder query = new();
        query.Append(_selectPart);
        query.Append(_fromPart);
        if(_wherePart.ToString() != "Where ") query.Append(_wherePart);
        if(_groupByPart.ToString() != "Group By ") query.Append(_groupByPart);
        if(_orderByPart.ToString() != "Order By ") query.Append(_orderByPart);
        if(_havingPart.ToString() != "Having ") query.Append(_havingPart);
        if(_limitPart.ToString() != "Limit ") query.Append(_limitPart);
        if(_offsetPart.ToString() != "Offset ")query.Append(_offsetPart);
        return query.ToString();
    }
}